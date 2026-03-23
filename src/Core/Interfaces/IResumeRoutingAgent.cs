using Core.Results;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface IResumeRoutingAgent
    {
        Task<ResumeRouteResult> DecideResumeRouteAsync(string userPrompt, CancellationToken cancellationToken = default);
    }
}
