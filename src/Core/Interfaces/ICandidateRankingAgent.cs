using Domain.Models;

namespace Core.Interfaces
{
    public interface ICandidateRankingAgent
    {
        Task<IReadOnlyList<CandidateMatch>> RankAsync(
            JobSearchQuery jobSearchQuery,
            IReadOnlyList<ResumeDocument> candidateResumes,
            CancellationToken cancellationToken = default);
    }
}
