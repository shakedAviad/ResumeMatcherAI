using Core.Interfaces;
using Core.Results;
using Microsoft.Agents.AI;

namespace Infrastructure.AI.Agents
{
    public class OpenAiResumeRoutingAgent : IResumeRoutingAgent
    {
        private readonly ChatClientAgent _agent;

        public OpenAiResumeRoutingAgent(ChatClientAgent agent)
        {
            _agent = agent;
        }

        public async Task<ResumeRouteResult> DecideResumeRouteAsync(string userPrompt, CancellationToken cancellationToken = default)
        {
            var prompt = $"User prompt: {userPrompt}";
            var resposne = await _agent.RunAsync<ResumeRouteResult>(prompt, cancellationToken: cancellationToken);
            var result = resposne.Result;

            return result switch
            {
                ResumeRouteResult => result,
                _ => throw new InvalidOperationException("Unexpected result type from the agent.")
            };
        }
    }
}
