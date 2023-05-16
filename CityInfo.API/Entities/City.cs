using CityInfo.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)] 
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)] 
        public string? Description { get; set; }
        public ICollection<PointofInterest> PointOfInterests 
        { get; set; } = new List<PointofInterest>();
        public City(string name)
        {
            Name = name;
        }
    }
}
