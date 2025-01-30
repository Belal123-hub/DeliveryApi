using System.Text.Json.Serialization;
namespace DTO
{
    public class LoginCredentialsDto
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
    public class RefreshCredentialsDto
    {
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
