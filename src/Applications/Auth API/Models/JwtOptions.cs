namespace Auth.API.Models
{
    public class JwtOptions
    {
        public string Issuer { get; init; } = "ResumeMatcherAI.Auth.API";
        public string Audience { get; init; } = "ResumeMatcherAI.Backend.API";
        public int ExpirationMinutes { get; init; } = 60;
    }
}
