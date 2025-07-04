﻿using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
	[ApiController]
	[Route("api/cities")]
	public class CitiesController : ControllerBase
	{
		[HttpGet]
		public ActionResult<IEnumerable<CityDto>> GetCities()
		{
			return Ok(CitiesDataStore.Current.Cities);
		}
		[HttpGet("{id}")]
		public ActionResult<CityDto> GetAllCity(int id)
		{
			var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);

			if (cityToReturn == null)
			{
				return NotFound();
			}

			return Ok(cityToReturn);

		}
	}
}
