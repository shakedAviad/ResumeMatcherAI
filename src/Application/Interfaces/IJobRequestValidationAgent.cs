using Domain.Models;

namespace Application.Interfaces
{
    public interface IJobRequestValidationAgent
    {
        Task<JobSearchQuery> ValidateAsync(string userPrompt, CancellationToken cancellationToken = default);
    }

}
