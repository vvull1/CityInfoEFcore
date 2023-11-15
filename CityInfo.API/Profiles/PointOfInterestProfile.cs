using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class PointOfInterestProfile : Profile
    {
        public PointOfInterestProfile()
        {
            CreateMap<Entities.PointofInterest, Models.PointofInterestDto>();
            CreateMap<Models.PointofInterestforCreatingDto, Entities.PointofInterest>();
            CreateMap<Models.PointofInterestForUpdateDto, Entities.PointofInterest>();
            CreateMap<Entities.PointofInterest, Models.PointofInterestForUpdateDto>();
        }
    }
}
