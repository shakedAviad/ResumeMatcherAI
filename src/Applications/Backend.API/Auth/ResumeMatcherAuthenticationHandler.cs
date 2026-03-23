using Backend.API.Auth.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Backend.API.Auth
{
    public class ResumeMatcherAuthenticationHandler : AuthenticationHandler<ResumeMatcherAuthenticationOptions>
    {
        private readonly IAuthValidationApiClient _authValidationApiClient;
        private readonly string _bearerPrefix = "Bearer ";
        public ResumeMatcherAuthenticationHandler(IOptionsMonitor<ResumeMatcherAuthenticationOptions> options, IAuthValidationApiClient authValidationApiClient, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
            _authValidationApiClient = authValidationApiClient;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorizationHeader = Request.Headers.Authorization.ToString();

            if (string.IsNullOrWhiteSpace(authorizationHeader))
            {
                return AuthenticateResult.NoResult();
            }

            if (!authorizationHeader.StartsWith(_bearerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Fail("Invalid authorization scheme.");
            }

            var accessToken = authorizationHeader[_bearerPrefix.Length..].Trim();
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return AuthenticateResult.Fail("Missing access token.");
            }

            var validationResult = await _authValidationApiClient.ValidateTokenAsync(accessToken, Context.RequestAborted);

            if (validationResult is null || !validationResult.IsValid)
            {
                return AuthenticateResult.Fail("Token validation failed.");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, validationResult.Subject),
                new(ClaimTypes.Name, validationResult.Subject)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
