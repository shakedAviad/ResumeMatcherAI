using Backend.API.Configuration;
using Core.Interfaces;
using Core.Workflows;
using Microsoft.Extensions.Options;

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
            var storage = scope.ServiceProvider.GetRequiredService<IOptions<StorageOptions>>().Value;
            var folderPath = storage.ResumeJsonDirectory;
            if (Directory.Exists(folderPath) && Directory.EnumerateFiles(folderPath).Any())
            {
                var resumeDocumentStore = scope.ServiceProvider.GetRequiredService<IResumeDocumentStore>();
                var resumeSearchIndex = scope.ServiceProvider.GetRequiredService<IResumeSearchIndex>();
                var resumes = await resumeDocumentStore.GetAllAsync(cancellationToken);

                foreach (var resume in resumes)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await resumeSearchIndex.IndexAsync(resume, cancellationToken);
                }
            }
            else
            {
                Directory.CreateDirectory(folderPath);
                var workflow = scope.ServiceProvider.GetRequiredService<ResumeIngestionWorkflow>();
                await workflow.ExecuteAsync(storage.ResumeFilesDirectory, cancellationToken);
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}