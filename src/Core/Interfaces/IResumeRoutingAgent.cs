using Core.Results;

namespace Core.Interfaces
{
    public interface IResumeRoutingAgent
    {
        Task<ResumeRouteResult> DecideResumeRouteAsync(string userPrompt, CancellationToken cancellationToken = default);
    }
}
