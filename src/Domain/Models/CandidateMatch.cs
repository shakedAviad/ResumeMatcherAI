namespace Domain.Models
{
    public class CandidateMatch
    {
        public string CandidateId { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public double Score { get; init; }
        public string Explanation { get; init; } = string.Empty;
        public int MatchScore { get; init; }
        public ResumeDocument? ResumeDocument { get; init; }
    }
}
