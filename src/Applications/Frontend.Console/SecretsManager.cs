using Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frontend.Console
{
    internal class SecretsManager
    {
        private readonly IConfiguration _configuration;

        public string OpenAiApiKey => _configuration["OpenAiApiKey"] ?? string.Empty;
        public string ChatModel => _configuration["ChatModel"] ?? string.Empty;
        public string BaseBackendAPIURL => SharedConfiguration.BaseBackendAPIURL ?? string.Empty;
        public SecretsManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }        
    }

}
