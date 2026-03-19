using Application.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public  class CandidateSearchService
    {
        private readonly IResumeSearchIndex _resumeSearchIndex;

        public CandidateSearchService(IResumeSearchIndex resumeSearchIndex)
        {
            _resumeSearchIndex = resumeSearchIndex;
        }

        public async Task<IReadOnlyList<ResumeDocument>> SearchAsync(JobSearchQuery jobSearchQuery, int maxResults, CancellationToken cancellationToken = default)
        {
            if (!jobSearchQuery.IsValid)
            {
                return [];
            }

            ArgumentNullException.ThrowIfNull(jobSearchQuery);

            if (maxResults <= 0) 
            {
                throw new ArgumentException($"Max results cannot be a negative number: {maxResults}");
            }

            return await _resumeSearchIndex.SearchAsync(jobSearchQuery, maxResults, cancellationToken);
        }
    }
}
