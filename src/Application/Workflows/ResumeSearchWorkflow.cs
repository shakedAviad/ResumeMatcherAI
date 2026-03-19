using Application.Commands;
using Application.Interfaces;
using Application.Results;
using Application.Services;

namespace Application.Workflows
{
    public class ResumeSearchWorkflow
    {
        private readonly IJobRequestValidationAgent _jobRequestValidationAgent;
        private readonly CandidateSearchService _candidateSearchService;
        private readonly ICandidateRankingAgent _candidateRankingAgent;

        public ResumeSearchWorkflow(IJobRequestValidationAgent jobRequestValidationAgent, CandidateSearchService candidateSearchService, ICandidateRankingAgent candidateRankingAgent)
        {
            _jobRequestValidationAgent = jobRequestValidationAgent;
            _candidateSearchService = candidateSearchService;
            _candidateRankingAgent = candidateRankingAgent;
        }

        public async Task<SearchCandidatesResult> ExecuteAsync(SearchCandidatesCommand command, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(command.UserPrompt);

            var jobSearchQuery = await _jobRequestValidationAgent.ValidateAsync(command.UserPrompt, cancellationToken);

            if (!jobSearchQuery.IsValid)
            {
                return new SearchCandidatesResult
                {
                    IsValidRequest = false,
                    Message = jobSearchQuery.ValidationMessage,
                    Candidates = []
                };
            }

            var candidateResumes = await _candidateSearchService.SearchAsync(jobSearchQuery, command.MaxResults, cancellationToken);

            if (candidateResumes.Count == 0)
            {
                return new SearchCandidatesResult
                {
                    IsValidRequest = true,
                    Message = "No matching candidates were found.",
                    Candidates = []
                };
            }

            var rankedCandidates = await _candidateRankingAgent.RankAsync(jobSearchQuery, candidateResumes, cancellationToken);

            return new SearchCandidatesResult
            {
                IsValidRequest = true,
                Message = rankedCandidates.Count == 0
                    ? "No matching candidates were found."
                    : "Candidates were found successfully.",
                Candidates = rankedCandidates
            };
        }
    }
}
