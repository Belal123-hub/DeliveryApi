using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
