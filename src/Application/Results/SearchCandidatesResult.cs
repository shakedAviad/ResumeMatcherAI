using Domain.Models;

namespace Application.Results
{
    public class SearchCandidatesResult
    {
        public bool IsValidRequest { get; init; }

        public string Message { get; init; } = string.Empty;

        public IReadOnlyList<CandidateMatch> Candidates { get; init; } = [];
    }
}
