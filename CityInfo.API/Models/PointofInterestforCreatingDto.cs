﻿using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models
{
    public class PointofInterestforCreatingDto
    {
        [Required(ErrorMessage = "You should provide a name value")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string? Description { get; set; }
    }
}
