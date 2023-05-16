using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); ;
        }
        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId,bool includePointofInterests)
        {
            if(includePointofInterests)
            {
                return await _context.Cities.Include(c => c.PointOfInterests)
                    .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }
            return await _context.Cities
                .Where(c=>c.Id== cityId).FirstOrDefaultAsync();
        }

        public async Task<PointofInterest?> GetPointofInterestForCityAsync(int cityId, int pointofInterestId)
        {
            return await _context.PointOfInterests
               .Where(p => p.CityId == cityId&&p.Id == pointofInterestId)
               .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PointofInterest>> GetPointofInterestsForCityAsync(int cityId)
        {
            return await _context.PointOfInterests
               .Where(p => p.CityId == cityId).ToListAsync();
        }
    }
}
