using Frontend.Console.Auth.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Frontend.Console.Auth.Handlers
{
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        private readonly IAuthApiClient _authApiClient;

        public JwtAuthorizationHandler(IAuthApiClient authApiClient)
        {
            _authApiClient = authApiClient;
        }

        protected override  Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authApiClient.AcessToken);

            return base.SendAsync(request, cancellationToken);
        }
    }
}
