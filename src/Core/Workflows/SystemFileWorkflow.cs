using Core.Commands;
using Core.Interfaces;
using Core.Results;

namespace Core.Workflows
{
    public class SystemFileWorkflow
    {
        private readonly IFileSystemAgent _fileSystemAgent;

        public SystemFileWorkflow(IFileSystemAgent fileSystemAgent)
        {
            _fileSystemAgent = fileSystemAgent;
        }

        public async Task<string> ExecuteAsync(BaseUserInputCommand command, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            return await _fileSystemAgent.EexcuteActAsync(command, cancellationToken);
        }
    }
}
