using Auth.API.Interfaces;
using Auth.API.Models;
using Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.API.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _options;
        private readonly IJwtSigningKeyProvider _signingKeyProvider;

        public JwtTokenService(IOptions<JwtOptions> options,IJwtSigningKeyProvider signingKeyProvider)
        {
            _options = options.Value;
            _signingKeyProvider = signingKeyProvider;
        }

        public TokenResponse CreateToken()
        {
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.ExpirationMinutes);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "ResumeMatcherAI.Frontend.Console"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),ClaimValueTypes.Integer64)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKeyProvider.Key));
            var signingCredentials = new SigningCredentials(signingKey,SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAtUtc,
                signingCredentials: signingCredentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenResponse
            {
                AccessToken = accessToken,
                ExpiresAtUtc = expiresAtUtc
            };
        }
        public ValidateTokenResponse ValidateToken(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return new ValidateTokenResponse { IsValid = false };
            }

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _options.Issuer,

                ValidateAudience = true,
                ValidAudience = _options.Audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKeyProvider.Key))
            };

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out _);
                var subject = GetSubject(principal);

                return new ValidateTokenResponse
                {
                    IsValid = true,
                    Subject = subject
                };
            }
            catch(Exception e)
            {
                return new ValidateTokenResponse { IsValid = false };
            }
        }

        private string GetSubject(ClaimsPrincipal principal) 
        {
            return principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? string.Empty;
        }
    }
}
