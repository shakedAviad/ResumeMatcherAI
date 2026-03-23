using Backend.API.Auth;
using Backend.API.Auth.Interfaces;
using Backend.API.Configuration;
using Backend.API.Startup;
using Core.Commands;
using Core.Interfaces;
using Core.Services;
using Core.Tools;
using Core.Workflows;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Office2016.Excel;
using Domain.Models;
using Infrastructure.AI.Agents;
using Infrastructure.AI.Prompts;
using Infrastructure.Documents;
using Infrastructure.Search.Indexs;
using Infrastructure.Search.Records;
using Infrastructure.Storage;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OpenAI;
using OpenAI.Chat;
using System.Reflection;

namespace Backend.API
{
    public static class Extensions
    {
        extension(WebApplicationBuilder builder) 
        {
            public WebApplicationBuilder ConfigureServices()
            {
                builder.WebHost.UseUrls(SharedConfiguration.BaseBackendAPIURL);
                builder.Services.CreateServices(builder.Configuration);

                return builder;
            }

            public WebApplication BuildApplication()
            {
                var app = builder.Build();
                
                if (app.Environment.IsDevelopment())
                {
                    app.MapOpenApi();
                    app.UseSwaggerUI(options =>options.SwaggerEndpoint("/openapi/v1.json", "ResumeMatcher API v1"));
                }
                
                app.UseAuthentication();
                app.UseAuthorization();

                //app.UseHttpsRedirection();
                app.MapResumeMatcherEndpoints();
                
                return app;
            }
        }

        extension(IServiceCollection services)
        {
            internal IServiceCollection CreateServices(IConfiguration configuration)
            {
                services.AddEndpointsApiExplorer()
                    .AddOpenApi()                    
                    .AddHostedServices()
                    .AddConfiguration(configuration)
                    .AddRAG()
                    .AddServices()
                    .AddAgents()
                    .AddWorkwflows()
                    .AddAuthentication("ResumeMatcher")
                    .AddScheme<ResumeMatcherAuthenticationOptions, ResumeMatcherAuthenticationHandler>("ResumeMatcher", options => { });
                
                services.AddAuthorization()
                    .AddHttpClient<IAuthValidationApiClient, AuthValidationApiClient>(client => client.BaseAddress = new Uri(SharedConfiguration.BaseBackendAuthAPIURL));
                return services;
            }
            private IServiceCollection AddHostedServices()
            {
                services.AddHostedService<ResumeIndexWarmupService>();

                return services;
            }

            private IServiceCollection AddConfiguration(IConfiguration configuration) 
            {
                services.Configure<OpenAiOptions>(configuration.GetSection(OpenAiOptions.SectionName));
                services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));

                return services;
            }

            private IServiceCollection AddRAG()
            {
                services.AddSingleton<IResumeSearchIndex>(sp =>
                {
                    var openAiOptions = sp.GetRequiredService<IOptions<OpenAiOptions>>().Value;
                    var client = new OpenAIClient(openAiOptions.ApiKey);
                    var embeddingClient = client.GetEmbeddingClient(openAiOptions.EmbeddingModel);
                    var embeddingGenerator = embeddingClient.AsIEmbeddingGenerator();
                    var vectorStore = new InMemoryVectorStore(new() { EmbeddingGenerator = embeddingGenerator });
                    var collection = vectorStore.GetCollection<string, ResumeVectorRecord>(ResumeVectorRecord.CollectionName);

                    return new InMemoryVectorResumeSearchIndex(collection);
                });

                return services;
            }

            private IServiceCollection AddServices() 
            {
                services.AddSingleton<IResumeTextExtractor, ResumeTextExtractor>();
                services.AddSingleton<IResumeDocumentStore>(sp =>
                {
                    var storageOptions = sp.GetRequiredService<IOptions<StorageOptions>>().Value;

                    return new ResumeJsonFileStore(storageOptions.ResumeJsonDirectory);
                });

                return services;
            }

            private IServiceCollection AddAgents()
            {
                services.AddTransient<IChatClient>(sp =>
                {
                    var openAiOptions = sp.GetRequiredService<IOptions<OpenAiOptions>>().Value;
                    var client = new OpenAIClient(openAiOptions.ApiKey);
                    var chatClient = client.GetChatClient(openAiOptions.ChatModel);

                    return chatClient.AsIChatClient();
                });

                services.AddSingleton<IResumeExtractionAgent>(sp =>
                {
                    var chatClient = sp.GetRequiredService<IChatClient>();

                    var agent = chatClient.AsAIAgent(new ChatClientAgentOptions
                    {
                        Name = "ResumeExtractionAgent",
                        ChatOptions = new()
                        {
                            Instructions = ResumeExtractionPrompt.Instructions
                        }
                    });

                    return new OpenAiResumeExtractionAgent(agent);
                });

                services.AddSingleton<IJobRequestValidationAgent>(sp =>
                {
                    var chatClient = sp.GetRequiredService<IChatClient>();

                    var agent = chatClient.AsAIAgent(new ChatClientAgentOptions
                    {
                        Name = "JobRequestValidationAgent",
                        ChatOptions = new()
                        {
                            Instructions = JobRequestValidationPrompt.Instructions

                            /* #pragma warning disable OPENAI001
                            RawRepresentationFactory = _ => new ChatCompletionOptions
                            {
                                ReasoningEffortLevel = ChatReasoningEffortLevel.Minimal
                            },*/
                        }
                    });

                    return new OpenAiJobRequestValidationAgent(agent);
                });

                services.AddSingleton<ICandidateRankingAgent>(sp =>
                {
                    var chatClient = sp.GetRequiredService<IChatClient>();

                    var agent = chatClient.AsAIAgent(new ChatClientAgentOptions
                    {
                        Name = "CandidateRankingAgent",
                        ChatOptions = new()
                        {
                            Instructions = CandidateRankingPrompt.Instructions
                        }
                    });

                    return new OpenAiCandidateRankingAgent(agent);
                });

                services.AddSingleton<IFileSystemAgent>(sp =>
                {
                    var chatClient = sp.GetRequiredService<IChatClient>();
                    var storageOptions = sp.GetRequiredService<IOptions<StorageOptions>>().Value;
                    var fileSystemTools = new FileSystemTools(storageOptions.ResumeFilesDirectory);
                    var methods = typeof(FileSystemTools).GetMethods(BindingFlags.Public | BindingFlags.Instance);

                    var agent = chatClient.AsAIAgent(new ChatClientAgentOptions
                    {
                        Name = "FileSystemAgent",
                        ChatOptions = new()
                        {
                            Instructions = FileSystemPrompt.Instructions,
                            Tools = methods.Select(x => AIFunctionFactory.Create(x, fileSystemTools)).Cast<AITool>().ToList()
                        }
                    });

                    return new OpenAiFileSystemAgent(agent);
                });
                return services;
            }

            private IServiceCollection AddWorkwflows() 
            {
                services.AddSingleton<CandidateSearchService>();
                services.AddSingleton<ResumeIngestionWorkflow>();
                services.AddSingleton<ResumeSearchWorkflow>();
                services.AddSingleton<SystemFileManageWorkflow>();

                return services;
            }
            
        }

        extension(IEndpointRouteBuilder builder)
        {
            internal IEndpointRouteBuilder MapResumeMatcherEndpoints()
            {
                var apiAuth = builder.MapGroup("/api").RequireAuthorization();

                apiAuth.MapGet("/resumes/ingest", IngestAsync).WithName("IngestResumes").WithTags("Resumes");
                apiAuth.MapPost("/candidates/search", SearchAsync).WithName("SearchCandidates").WithTags("Candidates");
                apiAuth.MapPost("/fileSystem/manage", ManageAsync).WithName("FileSystemManagement").WithTags("FileSystem");

                return builder;
            }

            private static async Task<IResult> IngestAsync(IServiceProvider provider, ResumeIngestionWorkflow workflow ,CancellationToken cancellationToken)
            {
                var storge = provider.GetRequiredService<IOptions<StorageOptions>>().Value;
                if (string.IsNullOrWhiteSpace(storge.ResumeFilesDirectory))
                {
                    return Results.BadRequest("Resume Files Directory is required.");
                }
                var result = await workflow.ExecuteAsync(storge.ResumeFilesDirectory, cancellationToken);

                return Results.Ok(result);
            }

            
            private static async Task<IResult> SearchAsync(BaseRequest request, ResumeSearchWorkflow workflow, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.UserPrompt))
                {
                    return Results.BadRequest("UserPrompt is required.");
                }

                var result = await workflow.ExecuteAsync(
                    new SearchCandidatesCommand
                    {
                        UserPrompt = request.UserPrompt,                        
                    },
                    cancellationToken);

                return Results.Ok(result);
            }
            private static async Task<IResult> ManageAsync(BaseRequest request, SystemFileManageWorkflow workflow, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.UserPrompt))
                {
                    return Results.BadRequest("UserPrompt is required.");
                }

                var result = await workflow.ExecuteAsync(
                    new BaseUserInputCommand
                    {
                        UserPrompt = request.UserPrompt
                    });

                return Results.Ok(result);
            }
        }

       
        public class BaseRequest
        {
            public string UserPrompt { get; set; } = string.Empty;
        }
        
    }
}

