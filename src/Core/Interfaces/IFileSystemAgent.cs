using Core.Commands;

namespace Core.Interfaces
{
    public interface IFileSystemAgent
    {
        Task<string> EexcuteActAsync(BaseUserInputCommand command, CancellationToken cancellationToken = default);
    }
}
