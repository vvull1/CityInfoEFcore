using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name,string? searchQuery,int pageNumber,int pageSize);
        Task<City?> GetCityAsync(int cityId, bool includePointofInterests);
        Task<bool> CityExistsAsync(int cityId);
        Task<IEnumerable<PointofInterest>> GetPointofInterestsForCityAsync(int cityId);
        Task<PointofInterest?> GetPointofInterestForCityAsync(int cityId, int pointofInterestId);
        Task AddPointOfInterestForCityAsync(int cityId, PointofInterest pointofInterest);
        void DeletePointofInterest(PointofInterest pointOfInterest);
        Task<bool> CityNameMatchesCityId(string? cityName,int cityId);
        Task<bool> SaveChangesAsync();
    }
}
