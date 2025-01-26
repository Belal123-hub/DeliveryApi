using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DTO
{
    public class UserDto
    {
        [JsonPropertyName("name")]
        [Required]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        [Required]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        [Required]
        public string Password { get; set; }

        [JsonPropertyName("birthDate")]
        public DateTime BirthDate { get; set; }
    }
}
