using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public sealed class JobSearchQuery
    {
        public string OriginalUserPrompt { get; init; } = string.Empty;

        public bool IsValid { get; init; }

        public string ValidationMessage { get; init; } = string.Empty;

        public string NormalizedRoleTitle { get; init; } = string.Empty;

        public IReadOnlyList<string> RequiredSkills { get; init; } = [];

        public IReadOnlyList<string> PreferredSkills { get; init; } = [];

        public IReadOnlyList<string> RequiredTechnologies { get; init; } = [];

        public IReadOnlyList<string> PreferredTechnologies { get; init; } = [];

        public IReadOnlyList<string> DomainKeywords { get; init; } = [];

        public string SeniorityLevel { get; init; } = string.Empty;

        public string Notes { get; init; } = string.Empty;
    }
}
