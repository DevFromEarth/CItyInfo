using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
	[Route("api/cities/{cityid}/pointsofinterest")]
	[ApiController]
	public class PointsOfInterestController : ControllerBase
	{
		[HttpGet]
		public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityid)
		{
			var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityid);

			if (city == null)
			{
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
					cityid = cityid,
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

			return NoContent();
		}
	}
}
