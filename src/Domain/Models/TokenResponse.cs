namespace Domain.Models
{
    public class TokenResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public DateTime ExpiresAtUtc { get; init; }
    }
}
