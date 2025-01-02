using DTO.Enums;
using System.ComponentModel.DataAnnotations;


namespace DTO
{
    public class DishDto
    {
        public int Id { get; set; } // UUID for the dish

        [Required]
        [MinLength(1)]
        public string Name { get; set; } // Dish name, required with a minimum length of 1

        public string? Description { get; set; } // Optional description of the dish

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        public double Price { get; set; } // Required price, must be a positive number

        public string? Image { get; set; } // Optional URL or path to the dish image

        public bool Vegetarian { get; set; } // Indicates if the dish is vegetarian

        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public double? Rating { get; set; } // Optional rating, ranges from 0 to 5
        public DishCategory? Category { get; set; }
    }
}
