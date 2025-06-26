using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
	.AddNewtonsoftJson();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
	Console.WriteLine("Before UseRouting:");
	Console.WriteLine($"Path: {context.Request.Path}");
	Console.WriteLine($"RouteValues: {string.Join(", ", context.Request.RouteValues.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
	Console.WriteLine();
	Console.WriteLine("1st is 1");
	await next();
	Console.WriteLine("1st is 2");
});


app.UseHttpsRedirection();

app.UseRouting();

app.Use(async (context, next) =>
{
	Console.WriteLine("After UseRouting, Before UseEndpoints:");
	Console.WriteLine($"Path: {context.Request.Path}");
	Console.WriteLine($"RouteValues: {string.Join(", ", context.Request.RouteValues.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");
	Console.WriteLine();
	Console.WriteLine("2nd is 1");
	await next();
	Console.WriteLine("2nd is 2");
});

app.UseAuthorization();

app.UseEndpoints(endpoints => 
{
	Console.WriteLine($"test: {endpoints}");
	endpoints.MapControllers(); 
});

app.Run();
