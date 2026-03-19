using Core.Commands;
using Core.Interfaces;
using Core.Results;

namespace Core.Workflows
{
    public class ResumeIngestionWorkflow
    {
        private readonly IResumeTextExtractor _resumeTextExtractor;
        private readonly IResumeExtractionAgent _resumeExtractionAgent;
        private readonly IResumeDocumentStore _resumeDocumentStore;
        private readonly IResumeSearchIndex _resumeSearchIndex;

        public ResumeIngestionWorkflow(IResumeTextExtractor resumeTextExtractor, IResumeExtractionAgent resumeExtractionAgent, IResumeDocumentStore resumeDocumentStore, IResumeSearchIndex resumeSearchIndex)
        {
            _resumeTextExtractor = resumeTextExtractor;
            _resumeExtractionAgent = resumeExtractionAgent;
            _resumeDocumentStore = resumeDocumentStore;
            _resumeSearchIndex = resumeSearchIndex;
        }

        public async Task<IngestResumesResult> ExecuteAsync(IngestResumesCommand command, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            if (command.FilePaths is null || command.FilePaths.Count == 0)
            {
                return new IngestResumesResult
                {
                    TotalFilesReceived = 0,
                    SuccessfullyProcessedCount = 0,
                    FailedCount = 0,
                    FailedFilePaths = []
                };
            }

            var failedFilePaths = command.FilePaths.Where(filePath => string.IsNullOrWhiteSpace(filePath)).ToList();

            foreach (var filePath in command.FilePaths.Where(filePath => !string.IsNullOrWhiteSpace(filePath)))
            {
                try
                {
                    var extractedText = await _resumeTextExtractor.ExtractTextAsync(filePath, cancellationToken);

                    if (string.IsNullOrWhiteSpace(extractedText))
                    {
                        failedFilePaths.Add(filePath);
                    }
                    else
                    {
                        var sourceFileName = Path.GetFileName(filePath);
                        var resumeDocument = await _resumeExtractionAgent.ExtractAsync(sourceFileName, extractedText, cancellationToken);

                        await _resumeDocumentStore.SaveAsync(resumeDocument, cancellationToken);
                        await _resumeSearchIndex.IndexAsync(resumeDocument, cancellationToken);
                    }

                }
                catch
                {
                    failedFilePaths.Add(filePath);
                }

            }

            return new IngestResumesResult
            {
                TotalFilesReceived = command.FilePaths.Count,
                SuccessfullyProcessedCount = command.FilePaths.Count - failedFilePaths.Count,
                FailedCount = failedFilePaths.Count,
                FailedFilePaths = failedFilePaths
            };
        }
    }
}
