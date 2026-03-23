using Core.Interfaces;
using Core.Results;
using Domain.Models;
using System.Net.Http.Json;

namespace Core.Workflows
{
    public class ResumeConversationWorkflow
    {
        private readonly IResumeConversationAgent _conversationAgent;
        private readonly IResumeRoutingAgent _routingAgent;
        private readonly HttpClient _httpClient;

        public ResumeConversationWorkflow(IResumeConversationAgent conversationAgent, IResumeRoutingAgent routingAgent, HttpClient httpClient)
        {
            _conversationAgent = conversationAgent;
            _routingAgent = routingAgent;
            _httpClient = httpClient;
        }

        public async Task<string> GetConversationResponse(string userPrompt, CancellationToken cancellationToken = default)
        {
            var routingResult = await _routingAgent.DecideResumeRouteAsync(userPrompt, cancellationToken);
            var responseMessage = routingResult.WorkflowType switch
            {
                ResumeWorkflowType.ResumeIngestion => await _httpClient.GetAsync(SharedConfiguration.ResumeIngestionEndpoint, cancellationToken),
                ResumeWorkflowType.ResumeSearch => await _httpClient.PostAsJsonAsync(SharedConfiguration.ResumeSearchEndpoint, new { UserPrompt = userPrompt }, cancellationToken),
                ResumeWorkflowType.SystemFile => await _httpClient.PostAsJsonAsync(SharedConfiguration.ResumeFileSystemEndpoint, new { UserPrompt = userPrompt }, cancellationToken),
                ResumeWorkflowType.Unknown => null,
                _ => throw new InvalidOperationException("Unable to determine the appropriate workflow for the given prompt."),
            };
            var messageContent = responseMessage is not null ? await responseMessage.Content.ReadAsStringAsync(cancellationToken) : "Unknown";
            var promptForConversation = $"""
                User Prompt:
                {userPrompt}
                
                WorkflowType:
                {routingResult.WorkflowType}
                
                MessageContent:
                {messageContent}
                """;
            var conversationResponse = await _conversationAgent.ReplyAsync(promptForConversation, cancellationToken);

            return conversationResponse;
        }
    }


}
