using Core.Commands;
using Core.Interfaces;
using Microsoft.Agents.AI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.AI.Agents
{

    public class OpenAiFileSystemAgent : IFileSystemAgent
    {
        private readonly ChatClientAgent _agent;

        public OpenAiFileSystemAgent(ChatClientAgent agent)
        {
            _agent = agent;
        }

        public async Task<string> EexcuteActAsync(BaseUserInputCommand command, CancellationToken cancellationToken = default)
        {
            var userPrompt = $"User Prompt {command.UserPrompt}";
            var response = await _agent.RunAsync<string>(userPrompt, cancellationToken: cancellationToken);

            return response.Result;

        }
    }
}
