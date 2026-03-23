
using Domain.Models;

namespace Auth.API.Interfaces
{
    public interface IJwtTokenService
    {
        TokenResponse CreateToken();
        ValidateTokenResponse ValidateToken(string accessToken);
    }
}
