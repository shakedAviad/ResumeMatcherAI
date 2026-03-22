using Core.Interfaces;
using Domain.Models;
using Microsoft.Agents.AI;

namespace Infrastructure.AI.Agents
{
    public class OpenAiJobRequestValidationAgent : IJobRequestValidationAgent
    {
        private readonly ChatClientAgent _agent;

        public OpenAiJobRequestValidationAgent(ChatClientAgent agent)
        {
            _agent = agent;
        }

        public async Task<JobSearchQuery> ValidateAsync(string userPrompt, CancellationToken cancellationToken = default)
        {
            var prompt = $"User prompt: {userPrompt}";
            var resposne = await _agent.RunAsync<JobSearchQuery>(prompt, cancellationToken: cancellationToken);
            var result = resposne.Result;

            return result switch
            {
                JobSearchQuery query => query,
                _ => throw new InvalidOperationException("LLM returned an invalid job search query.")
            };

        }
    }
}
