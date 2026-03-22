using Core.Commands;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface IFileSystemAgent
    {
        Task<string> EexcuteActAsync(BaseUserInputCommand command, CancellationToken cancellationToken = default);
    }
}
