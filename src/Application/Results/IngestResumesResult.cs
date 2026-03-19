using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Results
{
    public  class IngestResumesResult
    {
        public int TotalFilesReceived { get; init; }

        public int SuccessfullyProcessedCount { get; init; }

        public int FailedCount { get; init; }

        public IReadOnlyList<string> FailedFilePaths { get; init; } = [];
    }
}
