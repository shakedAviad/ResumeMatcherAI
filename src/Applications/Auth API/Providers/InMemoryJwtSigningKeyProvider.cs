using Auth.API.Interfaces;
using System.Security.Cryptography;

namespace Auth.API.Providers
{
    public class InMemoryJwtSigningKeyProvider : IJwtSigningKeyProvider
    {
        public string Key
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                {
                    var bytes = RandomNumberGenerator.GetBytes(64);

                    field = Convert.ToBase64String(bytes);
                }

                return field;
            }
        }
    }
}
