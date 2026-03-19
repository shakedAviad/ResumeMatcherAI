using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface ICandidateRankingAgent
    {
        Task<IReadOnlyList<CandidateMatch>> RankAsync(
            JobSearchQuery jobSearchQuery,
            IReadOnlyList<ResumeDocument> candidateResumes,
            CancellationToken cancellationToken = default);
    }
}
