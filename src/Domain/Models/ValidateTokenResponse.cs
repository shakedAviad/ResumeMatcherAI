namespace Domain.Models
{
    public class ValidateTokenResponse
    {
        public bool IsValid { get; init; }
        public string Subject { get; init; } = string.Empty;
    }
}
