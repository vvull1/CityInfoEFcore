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
        

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId);
        }


        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.OrderBy(c=>c.Name).ToListAsync();
        }

        public async Task<bool> CityNameMatchesCityId(string? cityName, int cityId)
        {
            return await _context.Cities.AnyAsync(c => c.Id == cityId && c.Name == cityName);
        }

        public async Task<(IEnumerable<City>,PaginationMetadata)> GetCitiesAsync(String? name,string? searchQuery,int pageNumber,int pageSize)
        {
            //if(string.IsNullOrEmpty(name)
            //    && string.IsNullOrWhiteSpace(searchQuery))
            //{
            //    return await GetCitiesAsync();
            //}
            var collection = _context.Cities as IQueryable<City>;
           
            if(!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection=collection.Where(c => c.Name == name);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(c => c.Name.Contains(searchQuery)
                || (c.Description!=null && c.Description.Contains(searchQuery)));
            }
            var totalItemCount = await collection.CountAsync();

            var paginationMetadata = new PaginationMetadata(
                totalItemCount, pageSize, pageNumber);
            var collectionToReturn=await collection.OrderBy(c => c.Name)
                .Skip(pageSize*(pageNumber-1))
                .Take(pageSize)
                .ToListAsync();

           return (collectionToReturn,paginationMetadata);
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

        public async Task AddPointOfInterestForCityAsync(int cityId, PointofInterest pointofInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if(city!=null)
            {
                city.PointOfInterests.Add(pointofInterest);
            }
        }

        public void DeletePointofInterest(PointofInterest pointofInterest)
        {
            _context.PointOfInterests.Remove(pointofInterest);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        
    }
}
