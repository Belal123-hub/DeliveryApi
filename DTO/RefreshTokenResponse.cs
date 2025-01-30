using System.ComponentModel.DataAnnotations;
namespace DTO
{
    public class RefreshTokenResponse
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
