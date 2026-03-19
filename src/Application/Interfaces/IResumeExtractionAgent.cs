using Domain.Models;

namespace Application.Interfaces
{
    public interface IResumeExtractionAgent
    {
        Task<ResumeDocument> ExtractAsync(string sourceFileName, string resumeText, CancellationToken cancellationToken = default);
    }
}
