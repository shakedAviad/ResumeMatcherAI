using Domain.Models;

namespace Core.Interfaces
{
    public interface IResumeDocumentStore
    {
        Task SaveAsync(ResumeDocument resumeDocument, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ResumeDocument>> GetAllAsync(CancellationToken cancellationToken = default);
        
    }
}
