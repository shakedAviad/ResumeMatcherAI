using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface IResumeConversationAgent
    {
        Task<string> ReplyAsync(string userPrompt, CancellationToken cancellationToken = default);
    }
}
