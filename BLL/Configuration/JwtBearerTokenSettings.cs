using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Configuration
{
    public class JwtBearerTokenSettings
    {
        public string secretKey {  get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int AccessTokenExpiryTimeInSeconds { get; set; }
        public int RefreshTokenExpiryTimeInDays { get; set; }
    }
}
