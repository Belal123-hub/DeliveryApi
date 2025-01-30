using System.ComponentModel.DataAnnotations;
namespace DTO
{
    public class TokenResponse
    {
        [Required]
        public string AccessToken { get; set; }
    }
}
