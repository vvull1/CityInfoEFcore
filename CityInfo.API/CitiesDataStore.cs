using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public List<CityDto> cities { get; set; }
       // public static CitiesDataStore current { get; } = new CitiesDataStore();
        public CitiesDataStore() 
        {
            cities = new List<CityDto>()
        {
            new CityDto()
            {
                Id= 1,
                Name="New York",
                Description="City of lights",
                PointOfInterests=new List<PointofInterestDto>
                {
                    new PointofInterestDto()
                    {
                        Id=1,
                        Name="Central park",
                        Description="Biggest Central park"
                    },
                    new PointofInterestDto()
                    {
                        Id=2,
                        Name="Time Square",
                        Description="Biggest Tower"
                    }
                }
            },
            new CityDto()
            {
                Id= 2,
                Name="New Jersey",
                Description="City of Indians",
                PointOfInterests=new List<PointofInterestDto>
                {
                    new PointofInterestDto()
                    {
                        Id=3,
                        Name="Central park",
                        Description="Biggest Central park"
                    },
                    new PointofInterestDto()
                    {
                        Id=4,
                        Name="Time Square",
                        Description="Biggest Tower"
                    }
                }
            },
            new CityDto()
            {
                Id= 3,
                Name="Connecticut",
                Description="City of Students",
                PointOfInterests=new List<PointofInterestDto>
                {
                    new PointofInterestDto()
                    {
                        Id=5,
                        Name="Central park",
                        Description="Biggest Central park"
                    },
                    new PointofInterestDto()
                    {
                        Id=6,
                        Name="Time Square",
                        Description="Biggest Tower"
                    }
                }
            }
        };
        }
    }
}
