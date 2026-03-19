namespace Domain.Models
{
    public class EmploymentRecord
    {
        public string CompanyName { get; init; } = string.Empty;

        public string RoleTitle { get; init; } = string.Empty;

        public string StartDateText { get; init; } = string.Empty;

        public string EndDateText { get; init; } = string.Empty;

        public string RoleSummary { get; init; } = string.Empty;
    }
}
