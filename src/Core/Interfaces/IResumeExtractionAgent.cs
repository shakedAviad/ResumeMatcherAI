using Domain.Models;

namespace Core.Interfaces
{
    public interface IResumeExtractionAgent
    {
        Task<ResumeDocument> ExtractAsync(string sourceFileName, string resumeText, CancellationToken cancellationToken = default);
    }
}
