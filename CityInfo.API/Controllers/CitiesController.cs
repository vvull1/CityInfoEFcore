using CityInfo.API.Models;
using CityInfo.API;
using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Services;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        
        private readonly ICityInfoRepository _cityInfoRepository;
        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> GetCities()
        {
            var cityEntities=await _cityInfoRepository.GetCitiesAsync();
            var results = new List<CityWithoutPointOfInterestDto>();
            foreach (var cityEntity in cityEntities)
            {
                results.Add(new CityWithoutPointOfInterestDto
                {
                    Id = cityEntity.Id,
                    Description = cityEntity.Description,
                    Name = cityEntity.Name

                });
            }
            return Ok(results);
        }
        //[HttpGet("{Id}")]
        //public ActionResult<CityDto> GetCity(int id)
        //{
        //    var citytoReturn=_citiesDataStore.cities.FirstOrDefault(c=>c.Id==id);
        //    if (citytoReturn == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(citytoReturn);

        //}
    }
}
