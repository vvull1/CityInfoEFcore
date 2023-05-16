using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<City?> GetCityAsync(int cityId, bool includePointofInterests);
        Task<IEnumerable<PointofInterest>> GetPointofInterestsForCityAsync(int cityId);
        Task<PointofInterest?> GetPointofInterestForCityAsync(int cityId, int pointofInterestId);
    }
}
