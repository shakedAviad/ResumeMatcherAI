using Domain.Models;

namespace Core.Interfaces
{
    public interface IJobRequestValidationAgent
    {
        Task<JobSearchQuery> ValidateAsync(string userPrompt, CancellationToken cancellationToken = default);
    }

}
