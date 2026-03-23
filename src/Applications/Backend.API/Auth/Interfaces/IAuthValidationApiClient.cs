using Domain.Models;

namespace Backend.API.Auth.Interfaces
{
    public interface IAuthValidationApiClient
    {
        Task<ValidateTokenResponse?> ValidateTokenAsync(string accessToken, CancellationToken cancellationToken = default);
    }
}
