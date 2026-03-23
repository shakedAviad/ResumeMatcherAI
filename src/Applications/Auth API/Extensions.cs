using Auth.API.Interfaces;
using Auth.API.Models;
using Auth.API.Providers;
using Auth.API.Services;
using Domain.Models;

namespace Auth.API
{
    public static class Extensions
    {
        extension(WebApplicationBuilder builder)
        {
            public WebApplicationBuilder ConfigureServices()
            {
                builder.WebHost.UseUrls(SharedConfiguration.BaseBackendAuthAPIURL);
                builder.Services.CreateServices(builder.Configuration);

                return builder;
            }

            public WebApplication BuildApplication()
            {
                var app = builder.Build();

                if (app.Environment.IsDevelopment())
                {
                    app.MapOpenApi();
                    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "ResumeMatcher API v1"));
                }

                app.MapResumeAuthEndpoints();

                return app;
            }
        }
        extension(IServiceCollection services)
        {
            internal IServiceCollection CreateServices(IConfiguration configuration)
            {
                services.AddEndpointsApiExplorer()
                    .AddOpenApi()
                    .AddConfiguration(configuration)
                    .AddServices();

                return services;
            }
            private IServiceCollection AddConfiguration(IConfiguration configuration)
            {
                services.Configure<JwtOptions>(configuration.GetSection("Jwt")); ;

                return services;
            }
            private IServiceCollection AddServices()
            {
                services.AddSingleton<IJwtSigningKeyProvider>(sp => new InMemoryJwtSigningKeyProvider());
                services.AddSingleton<IJwtTokenService, JwtTokenService>();
                return services;
            }
        }
        extension(IEndpointRouteBuilder endpoints)
        {
            internal IEndpointRouteBuilder MapResumeAuthEndpoints()
            {

                endpoints.MapGet("/api/auth/token", (IJwtTokenService jwtTokenService) =>
                {
                    var token = jwtTokenService.CreateToken();
                    return Results.Ok(token);
                });

                endpoints.MapPost("/api/auth/validate", (ValidateTokenRequest request, IJwtTokenService jwtTokenService) =>
                {
                    var result = jwtTokenService.ValidateToken(request.AccessToken);

                    return result.IsValid ? Results.Ok(result) : Results.Unauthorized();
                });

                return endpoints;
            }
        }


    }
}
