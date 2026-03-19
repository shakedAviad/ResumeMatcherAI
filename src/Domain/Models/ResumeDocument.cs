namespace Domain.Models
{

    public class ResumeDocument
    {
        public string CandidateId { get; set; } = string.Empty;

        public string SourceFileName { get; set; } = string.Empty;

        public string FullName { get; init; } = string.Empty;

        public string CurrentTitle { get; init; } = string.Empty;

        public int? YearsOfExperience { get; init; }

        public string SeniorityLevel { get; init; } = string.Empty;

        public string ProfessionalSummary { get; init; } = string.Empty;

        public IReadOnlyList<string> Skills { get; init; } = [];

        public IReadOnlyList<string> Technologies { get; init; } = [];

        public IReadOnlyList<string> Domains { get; init; } = [];

        public IReadOnlyList<string> EducationItems { get; init; } = [];

        public IReadOnlyList<string> SpokenLanguages { get; init; } = [];

        public IReadOnlyList<string> PreferredLocations { get; init; } = [];

        public IReadOnlyList<EmploymentRecord> EmploymentHistory { get; init; } = [];
    }
}
