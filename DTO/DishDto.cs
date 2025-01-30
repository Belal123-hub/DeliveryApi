using DTO.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class DishDto
    {
        public Guid Id { get; set; } 

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        public decimal Price { get; set; }

        public string? Image { get; set; }

        public bool Vegetarian { get; set; }

        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public double? Rating { get; set; }
        public DishCategory? Category { get; set; }
    }
}
