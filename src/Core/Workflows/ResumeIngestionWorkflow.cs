using Core.Interfaces;
using Core.Results;
using System.Collections.Concurrent;

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

        public async Task<IngestResumesResult> ExecuteAsync(string folderPath, CancellationToken cancellationToken = default)
        {
            var filePaths = Directory.GetFiles(folderPath);
            var failedFilePaths = new ConcurrentBag<string>(filePaths.Where(filePath => string.IsNullOrWhiteSpace(filePath)));

            await Parallel.ForEachAsync(filePaths.Where(filePath => !string.IsNullOrWhiteSpace(filePath)), async (filePath, cancellationToken) =>
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
                 catch (Exception ex)
                 {
                     failedFilePaths.Add(filePath);
                 }
             });

            return new IngestResumesResult
            {
                TotalFilesReceived = filePaths.Length,
                SuccessfullyProcessedCount = filePaths.Length - failedFilePaths.Count,
                FailedCount = failedFilePaths.Count,
                FailedFilePaths = [.. failedFilePaths]
            };
        }
    }
}
