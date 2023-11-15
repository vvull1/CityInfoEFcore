using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace CityInfo.API.Controllers
{
    [Route("api/v{version:apiVersion}/cities/{cityId}/pointsofInterest")]
    [Authorize(Policy="MustBeFromAntwerp")]
    [ApiVersion("2.0")]
    [ApiController]
    public class PointsofInterestController : ControllerBase
    {
        private readonly ILogger<PointsofInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;
        
        public PointsofInterestController(ILogger<PointsofInterestController> logger,
            IMailService mailService,ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointofInterestDto>>> GetPointofInterests(int cityId)
        {
            //var cityname=User.Claims.FirstOrDefault(c=>c.Type=="city")?.Value;

            //if(!await _cityInfoRepository.CityNameMatchesCityId(cityname,cityId))
            //{
            //    return Forbid();
            //}
            if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }
            var pointOfInterestForCity=await _cityInfoRepository.
                GetPointofInterestsForCityAsync(cityId);
            return Ok(_mapper.Map<IEnumerable<PointofInterestDto>>(pointOfInterestForCity));
             
        }

        [HttpGet("{pointofInterestId}", Name = "GetPointOfInterest")]
        public async Task<ActionResult<PointofInterestDto>> GetPointofInterestbyId(int cityId, int pointofInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointofInterest = await _cityInfoRepository.
                GetPointofInterestForCityAsync(cityId, pointofInterestId);
            if (pointofInterest == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PointofInterestDto>(pointofInterest));
        }

        [HttpPost]
        public async Task<ActionResult<PointofInterestDto>> CreatePointOfInterest(
           int cityId, PointofInterestforCreatingDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }
            var finalPointOfInterest = _mapper.Map<Entities.PointofInterest>(pointOfInterest);
            await _cityInfoRepository.AddPointOfInterestForCityAsync(
                cityId, finalPointOfInterest);

            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn =
                _mapper.Map<Models.PointofInterestDto>(finalPointOfInterest);


            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = finalPointOfInterest.Id
                },
                createdPointOfInterestToReturn);
        }

        [HttpPut("{pointofInterestId}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId,
            int pointofInterestId, PointofInterestForUpdateDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }
            var pointofInterestEntity= await _cityInfoRepository
                .GetPointofInterestForCityAsync(cityId, pointofInterestId);
            if (pointofInterestEntity == null)
            {
                return NotFound();
            }
            _mapper.Map(pointOfInterest, pointofInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();
           
            return NoContent();
        }

        [HttpPatch("{pointofinterestid}")]
        public async Task<ActionResult> PartiallyUpdatePointOfInterest(
            int cityId, int pointOfInterestId,
            JsonPatchDocument<PointofInterestForUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation(
                    $"City with id {cityId} wasn't found when accessing points of interest.");
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointofInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointofInterestForUpdateDto>(
                pointOfInterestEntity);

            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{pointofInterestId}")]
        public async Task<ActionResult> DeletePointofInterest(int cityId, int pointofInterestId)
        {
            if(!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointofInterestForCityAsync(cityId, pointofInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointofInterest(pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            _mailService.Send(
                "Point of interest deleted.",
                $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();

        }

    }
}
