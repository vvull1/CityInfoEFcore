namespace CityInfo.API.Models
{
    public class CityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int NumberofPointsofInterest
        {
           get {
                return PointOfInterests.Count;
            }
        }
        public ICollection<PointofInterestDto> PointOfInterests { get; set;} = new List<PointofInterestDto>();

    }
}
