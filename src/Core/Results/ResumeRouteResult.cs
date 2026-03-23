namespace Core.Results
{
    public class ResumeRouteResult
    {
        public ResumeWorkflowType WorkflowType { get; init; }
    }
    public enum ResumeWorkflowType
    {
        Unknown = 0,
        ResumeIngestion = 1,
        ResumeSearch = 2,
        SystemFile = 3
    }
}
