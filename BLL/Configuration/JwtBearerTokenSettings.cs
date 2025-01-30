namespace BLL.Configuration
{
    public class JwtBearerTokenSettings
    {
        public string secretKey {  get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int AccessTokenExpiryTimeInSeconds { get; set; }
        public double RefreshTokenExpiryTimeInDays { get; set; }
    }
}
