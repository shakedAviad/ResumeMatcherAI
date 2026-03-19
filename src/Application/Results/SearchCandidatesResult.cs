using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Results
{
    public  class SearchCandidatesResult
    {
        public bool IsValidRequest { get; init; }

        public string Message { get; init; } = string.Empty;

        public IReadOnlyList<CandidateMatch> Candidates { get; init; } = [];
    }
}
