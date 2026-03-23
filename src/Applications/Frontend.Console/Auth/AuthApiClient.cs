
using Domain.Models;
using Frontend.Console.Auth.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Frontend.Console.Auth
{
    public class AuthApiClient : IAuthApiClient
    {
        private readonly HttpClient _httpClient;

        public string AcessToken 
        { 
            get 
            {
                if (string.IsNullOrWhiteSpace(field)) 
                {
                    field = GetAccessTokenAsync().GetAwaiter().GetResult();
                }

                return field;
            } 
        }

        public AuthApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync("/api/auth/token", cancellationToken);
            
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken);
           
            return token is null || string.IsNullOrWhiteSpace(token.AccessToken) ? 
                throw new InvalidOperationException("Failed to get access token.") :
                token.AccessToken;
        }
    }
}
