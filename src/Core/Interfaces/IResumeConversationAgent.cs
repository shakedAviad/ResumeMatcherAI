namespace Core.Interfaces
{
    public interface IResumeConversationAgent
    {
        Task<string> ReplyAsync(string userPrompt, CancellationToken cancellationToken = default);
    }
}
