using Core.Interfaces;
using Domain.Models;
using Microsoft.Agents.AI;

namespace Infrastructure.AI.Agents
{

    public class OpenAiResumeExtractionAgent : IResumeExtractionAgent
    {
        private readonly ChatClientAgent _agent;

        public OpenAiResumeExtractionAgent(ChatClientAgent agent)
        {
            _agent = agent;
        }

        public async Task<ResumeDocument> ExtractAsync(string sourceFileName, string resumeText, CancellationToken cancellationToken = default)
        {
            var prompt = $"Given resume: {resumeText}";
            var response = await _agent.RunAsync<ResumeDocument>(prompt, cancellationToken: cancellationToken);
            var resume = response?.Result;

            return resume switch
            {
                ResumeDocument => resume,
                _ => throw new InvalidOperationException("LLM returned null."),
            };
        }
    }
}
