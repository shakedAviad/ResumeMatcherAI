using Backend.API.Auth.Interfaces;
using Domain.Models;
using System.Net;

namespace Backend.API.Auth
{
    public class AuthValidationApiClient : IAuthValidationApiClient
    {
        private readonly HttpClient _httpClient;

        public AuthValidationApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ValidateTokenResponse?> ValidateTokenAsync(string accessToken, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "/api/auth/validate",
                new ValidateTokenRequest
                {
                    AccessToken = accessToken
                },
                cancellationToken);

            if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
            {
                return new ValidateTokenResponse
                {
                    IsValid = false
                };
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ValidateTokenResponse>(cancellationToken);
        }
    }
}
