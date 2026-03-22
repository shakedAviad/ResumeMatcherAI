using Backend.API.Configuration;
using Backend.API.Startup;
using Core.Commands;
using Core.Interfaces;
using Core.Services;
using Core.Tools;
using Core.Workflows;
using DocumentFormat.OpenXml.Office2010.CustomUI;
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
        extension(IServiceCollection services)
        {
            public IServiceCollection AddResumeMatcherServices(IConfiguration configuration)
            {
                services.AddHostedService<ResumeIndexWarmupService>();

                services.Configure<OpenAiOptions>(configuration.GetSection(OpenAiOptions.SectionName));
                services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));

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

                services.AddSingleton<IResumeTextExtractor, ResumeTextExtractor>();
                services.AddSingleton<IResumeDocumentStore>(sp =>
                {
                    var storageOptions = sp.GetRequiredService<IOptions<StorageOptions>>().Value;

                    return new ResumeJsonFileStore(storageOptions.ResumeJsonDirectory);
                });

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

                services.AddSingleton<CandidateSearchService>();
                services.AddSingleton<ResumeIngestionWorkflow>();
                services.AddSingleton<ResumeSearchWorkflow>();
                services.AddSingleton<SystemFileWorkflow>();

                return services;
            }
        }

        extension(IEndpointRouteBuilder endpoints)
        {
            public IEndpointRouteBuilder MapResumeMatcherEndpoints()
            {
                endpoints.MapGet("/", () => Results.Ok("ResumeMatcher API is running."));
                endpoints.MapResumeIngestionEndpoints();
                endpoints.MapCandidateSearchEndpoints();
                endpoints.MapFileSystemManageEndpoints();

                return endpoints;
            }

            private IEndpointRouteBuilder MapResumeIngestionEndpoints()
            {
                endpoints.MapPost("/api/resumes/ingest", IngestAsync)
                .WithName("IngestResumes")
                .WithTags("Resumes");

                return endpoints;
            }
            private IEndpointRouteBuilder MapCandidateSearchEndpoints()
            {
                endpoints.MapPost("/api/candidates/search", SearchAsync)
                .WithName("SearchCandidates")
                .WithTags("Candidates");

                return endpoints;
            }

            private IEndpointRouteBuilder MapFileSystemManageEndpoints() 
            {
                endpoints.MapPost("/api/fileSystem/manage", ManageAsync)
                .WithName("FileSystemManagement")
                .WithTags("FileSystem");

                return endpoints;

            }
            private static async Task<IResult> IngestAsync(IngestResumesRequest request, ResumeIngestionWorkflow workflow, CancellationToken cancellationToken)
            {
                var command = new IngestResumesCommand
                {
                    FolderPath = request.FolderPath
                };

                var result = await workflow.ExecuteAsync(command, cancellationToken);

                return Results.Ok(result);
            }

            
            private static async Task<IResult> SearchAsync(SearchCandidatesRequest request, ResumeSearchWorkflow workflow, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.UserPrompt))
                {
                    return Results.BadRequest("UserPrompt is required.");
                }

                var result = await workflow.ExecuteAsync(
                    new SearchCandidatesCommand
                    {
                        UserPrompt = request.UserPrompt,
                        MaxResults = request.MaxResults
                    },
                    cancellationToken);

                return Results.Ok(result);
            }
        }

        private static async Task<IResult> ManageAsync(BaseRequest request, SystemFileWorkflow workflow, CancellationToken cancellationToken)
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
        public class BaseRequest
        {
            public string UserPrompt { get; set; } = string.Empty;
        }
        public class IngestResumesRequest
        {
            public string FolderPath { get; set; } = string.Empty;
        }
        public class SearchCandidatesRequest : BaseRequest
        {            
            public int MaxResults { get; set; } = 10;
        }
    }
}

