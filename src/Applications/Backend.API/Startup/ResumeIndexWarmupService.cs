using Core.Interfaces;

namespace Backend.API.Startup
{
    public class ResumeIndexWarmupService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ResumeIndexWarmupService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var resumeDocumentStore = scope.ServiceProvider.GetRequiredService<IResumeDocumentStore>();
            var resumeSearchIndex = scope.ServiceProvider.GetRequiredService<IResumeSearchIndex>();
            var resumes = await resumeDocumentStore.GetAllAsync(cancellationToken);

            foreach (var resume in resumes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await resumeSearchIndex.IndexAsync(resume, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}