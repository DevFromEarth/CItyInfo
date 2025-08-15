using Microsoft.AspNetCore.StaticFiles;
using Serilog;

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.WriteTo.Console()
	.WriteTo.File("logs/cityinfo.txt",rollingInterval: RollingInterval.Day)
	.CreateLogger();

var builder = WebApplication.CreateBuilder(args);
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
builder.Host.UseSerilog();

builder.Services.AddProblemDetails();

//builder.Services.AddProblemDetails(options =>
//{
//	options.CustomizeProblemDetails = ctx =>
//	{
//		ctx.ProblemDetails.Extensions.Add("additionalInfo", "This is a additional info");
//		ctx.ProblemDetails.Extensions.Add("server", Environment.MachineName);
//	};
//});

// Add services to the container.
builder.Services.AddControllers(options =>
{
	options.ReturnHttpNotAcceptable = true;
}).AddXmlDataContractSerializerFormatters().AddNewtonsoftJson();
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler();
}

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
