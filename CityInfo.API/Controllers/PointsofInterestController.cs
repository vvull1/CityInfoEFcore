using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofInterest")]
    [ApiController]
    public class PointsofInterestController : ControllerBase
    {
        private readonly ILogger<PointsofInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly CitiesDataStore _citiesDataStore;
        public PointsofInterestController(ILogger<PointsofInterestController> logger,IMailService mailService,CitiesDataStore citiesDataStore)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));


        }
        [HttpGet]
        public ActionResult<IEnumerable<PointofInterestDto>> GetPointofInterests(int cityId)
        {
            try
            {
               // throw new Exception("Hi");
                var citytoReturn = _citiesDataStore.cities.FirstOrDefault(c => c.Id == cityId);
                if (citytoReturn == null)
                {
                    _logger.LogInformation(
                        $"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound();
                }
                return Ok(citytoReturn.PointOfInterests);
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"Exception while getting city {cityId}",ex);
                return StatusCode(500, "A problem occured while handling request");
            }
            
        }

        [HttpGet("{pointofInterestId}", Name = "GetPointOfInterest")]
        public ActionResult<PointofInterestDto> GetPointofInterestbyId(int cityId, int pointofInterestId)
        {

            var city = _citiesDataStore.cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointofInterest = city.PointOfInterests.FirstOrDefault(c => c.Id == pointofInterestId);
            if (pointofInterest == null)
            {
                return NotFound();
            }
            return Ok(pointofInterest);
        }

        [HttpPost]
        public ActionResult<PointofInterestDto> CreatePointOfInterest(
           int cityId, PointofInterestforCreatingDto pointOfInterest)
        {
            var city = _citiesDataStore.cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var maxPointofInterestId = _citiesDataStore.cities.SelectMany(c => c.PointOfInterests).Max(p => p.Id);
            var finalPointOfInterest = new PointofInterestDto()
            {
                Id = ++maxPointofInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };
            city.PointOfInterests.Add(finalPointOfInterest);


            return CreatedAtRoute("GetPointOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfInterestId = finalPointOfInterest.Id
                },
                finalPointOfInterest);
        }

        [HttpPut("{pointofInterestId}")]
        public ActionResult UpdatePointOfInterest(int cityId, int pointofInterestId, PointofInterestForUpdateDto pointOfInterest)
        {
            var city = _citiesDataStore.cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var pointofInterestfromStore = city.PointOfInterests.FirstOrDefault(c => c.Id == pointofInterestId);
            if (pointofInterestfromStore == null)
            {
                return NotFound();
            }
            pointofInterestfromStore.Name = pointOfInterest.Name;
            pointofInterestfromStore.Description = pointOfInterest.Description;
            return NoContent();
        }

        [HttpPatch("{pointofinterestid}")]
        public ActionResult PartiallyUpdatePointOfInterest(
            int cityId, int pointOfInterestId,
            JsonPatchDocument<PointofInterestForUpdateDto> patchDocument)
        {
            var city = _citiesDataStore.cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var pointofInterestfromStore = city.PointOfInterests.FirstOrDefault(c => c.Id == pointOfInterestId);
            if (pointofInterestfromStore == null)
            {
                return NotFound();
            }
            var pointofInteresttoPatch = new PointofInterestForUpdateDto()
            {
                Name = pointofInterestfromStore.Name,
                Description = pointofInterestfromStore.Description
            };
            patchDocument.ApplyTo(pointofInteresttoPatch,ModelState);
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!TryValidateModel(pointofInteresttoPatch))
            {
                return BadRequest(ModelState);
            }
            pointofInterestfromStore.Name=pointofInterestfromStore.Name;
            pointofInterestfromStore.Description = pointofInterestfromStore.Description;
            return NoContent();
        }
        [HttpDelete("{pointofInterestId}")]
        public ActionResult DeletePointofInterest(int cityId,int pointofInterestId)
        {
            var city = _citiesDataStore.cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var pointofInterestfromStore = city.PointOfInterests.FirstOrDefault(c => c.Id == pointofInterestId);
            if (pointofInterestfromStore == null)
            {
                return NotFound();
            }
            city.PointOfInterests.Remove(pointofInterestfromStore);
            _mailService.Send("Point f interest deleted.",$"point of Interest {pointofInterestfromStore.Name} with id {pointofInterestfromStore.Id} was deleted");
            return NoContent();

        }

    }
}
