using Core.Interfaces;
using Microsoft.Agents.AI;

namespace Infrastructure.AI.Agents
{
    public class OpenAiResumeConversationAgent : IResumeConversationAgent
    {
        private readonly ChatClientAgent _agent;
        private readonly AgentSession _session;

        public OpenAiResumeConversationAgent(ChatClientAgent agent)
        {
            _agent = agent;
            _session = agent.CreateSessionAsync().GetAwaiter().GetResult();
        }

        public async Task<string> ReplyAsync(string userPrompt, CancellationToken cancellationToken = default)
        {
            var prompt = $"User prompt: {userPrompt}";
            var response = await _agent.RunAsync<string>(prompt, session: _session, cancellationToken: cancellationToken);

            try
            {
                return response.Result;
            }
            catch (Exception ex)
            {
                return await ReplyAsync("System error occurred", cancellationToken);
            }
        }
    }
}
