using Core.Interfaces;
using Domain.Models;
using System.Text.Json;

namespace Infrastructure.Storage
{
    public class ResumeJsonFileStore : IResumeDocumentStore
    {
        private readonly string _directoryPath;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public ResumeJsonFileStore(string directoryPath)
        {
            _directoryPath = directoryPath;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task SaveAsync(ResumeDocument resumeDocument, CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(_directoryPath);

            var filePath = BuildFilePath(resumeDocument.CandidateId);
            var json = JsonSerializer.Serialize(resumeDocument, _jsonSerializerOptions);

            await File.WriteAllTextAsync(filePath, json, cancellationToken);
        }

        public async Task<IReadOnlyList<ResumeDocument>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var filePaths = Directory.GetFiles(_directoryPath, "*.json", SearchOption.TopDirectoryOnly);
            var resumes = new List<ResumeDocument>();

            foreach (var filePath in filePaths)
            {
                var json = await File.ReadAllTextAsync(filePath, cancellationToken);
                var resumeDocument = JsonSerializer.Deserialize<ResumeDocument>(json, _jsonSerializerOptions);

                resumes.Add(resumeDocument!);
            }

            return resumes;
        }

        private string BuildFilePath(string candidateId)
        {
            var safeFileName = string.Concat(candidateId.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));

            return Path.Combine(_directoryPath, $"{safeFileName}.json");
        }
    }
}
