using CityInfo.API.Models;
using CityInfo.API;
using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Services;
using AutoMapper;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly IMapper _mapper; 
        private readonly ICityInfoRepository _cityInfoRepository;
        const int maxCitiesPageSize = 20;
        public CitiesController(ICityInfoRepository cityInfoRepository,IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository;
            _mapper = mapper;   
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointOfInterestDto>>> 
            GetCities([FromQuery]string? name,string? searchQuery,int pageNumber=1,int pageSize=10)
        {
            if(pageSize>maxCitiesPageSize)
            {
                pageSize=maxCitiesPageSize;
            }
            var (cityEntities,paginationMetadata)=await _cityInfoRepository
                .GetCitiesAsync(name, searchQuery,pageNumber,pageSize);

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cityEntities)); //using automapper
            //var results = new List<CityWithoutPointOfInterestDto>();//without using automapper
            //foreach (var cityEntity in cityEntities)
            //{
            //    results.Add(new CityWithoutPointOfInterestDto
            //    {
            //        Id = cityEntity.Id,
            //        Description = cityEntity.Description,
            //        Name = cityEntity.Name

            //    });
            //}
            //return Ok(results);
        }
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetCity
            (int id,bool includePointOfInterest=false)
        {
            var citytoReturn = await _cityInfoRepository.GetCityAsync(id, includePointOfInterest);
            if (citytoReturn == null) 
            {
                return NotFound();
            }
            if(includePointOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(citytoReturn));
            }
            return Ok(_mapper.Map<CityWithoutPointOfInterestDto>(citytoReturn));

        }
    }
}
