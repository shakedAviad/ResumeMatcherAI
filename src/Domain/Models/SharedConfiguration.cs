namespace Domain.Models
{
    public class SharedConfiguration
    {
        public const string BaseBackendAPIURL = "http://localhost:5172";
        public const string BaseBackendAuthAPIURL = "http://localhost:5212";
        public const string ResumeIngestionEndpoint = "/api/resumes/ingest";
        public const string ResumeSearchEndpoint = "/api/candidates/search";

        public const string ResumeFileSystemEndpoint = "/api/fileSystem/manage";
    }
}
