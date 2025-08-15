using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
	[Route("api/cities/{cityid}/pointsofinterest")]
	[ApiController]
	public class PointsOfInterestController : ControllerBase
	{
		private readonly ILogger<PointsOfInterestController> _logger;
		private readonly IMailService _mailService;

		public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
		}

		[HttpGet]
		public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityid)
		{
			////this is not a right approach since it is not a global approach. we cant keep adding try and catch at every place
			//try
			//{
			//	throw new Exception("demo ex");
			//	var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityid);

			//	if (city == null)
			//	{
			//		_logger.LogInformation($"City with id {cityid} wasn't found.");
			//		return NotFound();
			//	}

			//	return Ok(city.PointsOfInterest);
			//}
			//catch (Exception ex)
			//{
			//	_logger.LogCritical($"Extion while getting points of interest for city with id {cityid}", ex);
			//	return StatusCode(500, "A problem happened while handling your request");
			//}

			//throw new Exception("demo ex");
			_logger.LogTrace("trace msg");
			_logger.LogDebug("debug msg");
			_logger.LogWarning("warning msg");
			_logger.LogError("error msg");
			_logger.LogCritical("critical msg");
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityid);

			if (city == null)
			{
				_logger.LogInformation($"City with id {cityid} wasn't found.");
				return NotFound();
			}

			return Ok(city.PointsOfInterest);
		}

		[HttpGet("{pointsofinterestid}" ,Name = "GetPointOfInterest")]
		public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityid, int pointsofinterestid)
		{
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityid);

			if (city == null)
			{
				return NotFound();
			}

			var pointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointsofinterestid);


			if (pointOfInterest == null)
			{
				return NotFound();
			}

			return Ok(pointOfInterest);
		}

		[HttpPost]
		public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityid, [FromBody]  PointOfInterestForCreationDto pointOfInterest)
		{
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityid);

			if (city == null)
			{
				return NotFound();
			}

			var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);

			var finalPointOfInterest = new PointOfInterestDto()
			{
				Id = ++maxPointOfInterestId,
				Name = pointOfInterest.Name,
				Description = pointOfInterest.Description
			};

			city.PointsOfInterest.Add(finalPointOfInterest);

			var response = new
			{
				Id = 9999,
				Name = "test"
			};

			return CreatedAtRoute("GetPointOfInterest",
				new
				{
					cityid = 1,
					pointsofinterestid = finalPointOfInterest.Id
				},
				response);

		}

		[HttpPut("{pointsofinterestid}")]
		public ActionResult UpdatePointOfInterest(int cityid, int pointsofinterestid, PointOfInterestForUpdateDto pointOfInterest)
		{
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityid);
			if (city == null)
			{
				return NotFound();
			}
			
			var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointsofinterestid);
			if (pointOfInterestFromStore == null)
			{ 
				return NotFound(); 
			}

			pointOfInterestFromStore.Name = pointOfInterest.Name;
			pointOfInterestFromStore.Description = pointOfInterest.Description;

			return NoContent();
		}

		[HttpPatch("{pointofinterestid}")]
		public ActionResult PartiallyUpdatePointOfInterest (int cityId, int pointofinterestid, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
		{
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null)
			{
				return NotFound();
			}

			var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointofinterestid);
			if (pointOfInterestFromStore == null)
			{
				return NotFound();
			}

			var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
			{
				Name = pointOfInterestFromStore.Name,
				Description = pointOfInterestFromStore.Description
			};

			patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if(!TryValidateModel(pointOfInterestToPatch))
			{
				return BadRequest(ModelState);
			}

			pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
			pointOfInterestFromStore.Description += pointOfInterestToPatch.Description;

			return NoContent();
		}

		[HttpDelete("{pointofinterestid}")]
		public ActionResult DeletePointOfInterest(int cityId, int pointofinterestid)
		{
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
			if (city == null)
			{
				return NotFound();
			}

			var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(c => c.Id == pointofinterestid);
			if (pointOfInterestFromStore == null)
			{
				return NotFound();
			}
			city.PointsOfInterest.Remove(pointOfInterestFromStore);
			_mailService.Send(
				"Point of interest deleted.",
				$"Point of interest {pointOfInterestFromStore.Name} with id {pointOfInterestFromStore.Id} was deleted.");

			return NoContent();
		}
	}
}
