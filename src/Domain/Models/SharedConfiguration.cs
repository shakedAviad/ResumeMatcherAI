using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class SharedConfiguration
    {
        public const string BaseBackendAPIURL = "http://localhost:5001";
        public const string ResumeIngestionEndpoint = "/api/resumes/ingest";
        public const string ResumeSearchEndpoint = "/api/candidates/search";
        
        public const string ResumeFileSystemEndpoint = "/api/fileSystem/manage";
    }
}
