using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public sealed class CandidateMatch
    {
        public string CandidateId { get; init; } = string.Empty;

        public string FullName { get; init; } = string.Empty;

        public string CurrentTitle { get; init; } = string.Empty;

        public double MatchScore { get; init; }

        public string ShortExplanation { get; init; } = string.Empty;
    }
}
