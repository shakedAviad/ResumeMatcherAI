using Core.Interfaces;
using Core.Workflows;
using Domain.Models;
using Frontend.Console.Auth;
using Frontend.Console.Auth.Handlers;
using Frontend.Console.Auth.Interfaces;
using Infrastructure.AI.Agents;
using Infrastructure.AI.Prompts;
using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frontend.Console
{
    public static class Extensions
    {
        extension(HostApplicationBuilder builder) 
        {
            public HostApplicationBuilder ConfigureServices()
            {                
                builder.Services.CreateServices(builder.Configuration);

                return builder;
            }
            public HostApplicationBuilder BuildApplication() 
            {
                builder.Logging.ClearProviders();
                builder.Logging.AddConsole();
                builder.Logging.SetMinimumLevel(LogLevel.Warning);

                return builder;
            }
            
            public IServiceProvider CreateProvider() 
            {
                return builder.Build().Services;
            }
        }
        extension(IServiceCollection services) 
        {
            internal IServiceCollection CreateServices(ConfigurationManager configuration) 
            {
                configuration.AddUserSecrets<SecretsManager>(optional: true);

                services.AddSingleton<SecretsManager>();

                services.AddTransient<JwtAuthorizationHandler>();

                services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
                {
                    client.BaseAddress = new Uri(SharedConfiguration.BaseBackendAuthAPIURL);
                });

                services.AddTransient<IChatClient>(sp =>
                {
                    var openAiOptions = sp.GetRequiredService<SecretsManager>();
                    var client = new OpenAIClient(openAiOptions.OpenAiApiKey);
                    var chatClient = client.GetChatClient(openAiOptions.ChatModel);

                    return chatClient.AsIChatClient();
                });

                services.AddSingleton<IResumeRoutingAgent>(sp =>
                {
                    var chatClient = sp.GetRequiredService<IChatClient>();

                    var agent = chatClient.AsAIAgent(new ChatClientAgentOptions
                    {
                        Name = "ResumeRoutingAgent",
                        ChatOptions = new()
                        {
                            Instructions = ResumeRoutingPrompt.Instructions
                        }
                    });

                    return new OpenAiResumeRoutingAgent(agent);
                });

                services.AddScoped<IResumeConversationAgent>(sp =>
                {
                    var chatClient = sp.GetRequiredService<IChatClient>();

                    var agent = chatClient.AsAIAgent(new ChatClientAgentOptions
                    {
                        Name = "ResumeConversationAgent",
                        ChatOptions = new()
                        {
                            Instructions = ResumeConversationPrompt.Instructions,
                        },

                    });

                    return new OpenAiResumeConversationAgent(agent);
                });

                services.AddHttpClient<ResumeConversationWorkflow>((sp, client) =>
                {
                    client.BaseAddress = new Uri(sp.GetRequiredService<SecretsManager>().BaseBackendAPIURL);
                }).AddHttpMessageHandler<JwtAuthorizationHandler>();

                return services;
            }
        }
    }
}
