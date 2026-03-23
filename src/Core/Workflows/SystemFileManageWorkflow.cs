using Core.Commands;
using Core.Interfaces;

namespace Core.Workflows
{
    public class SystemFileManageWorkflow
    {
        private readonly IFileSystemAgent _fileSystemAgent;

        public SystemFileManageWorkflow(IFileSystemAgent fileSystemAgent)
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
