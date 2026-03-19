using Domain.Models;

namespace Application.Interfaces
{
    public interface IResumeSearchIndex
    {
        Task IndexAsync(ResumeDocument resumeDocument, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ResumeDocument>> SearchAsync(JobSearchQuery jobSearchQuery, int maxResults, CancellationToken cancellationToken = default);
    }
}
