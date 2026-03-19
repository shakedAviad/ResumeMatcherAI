using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands
{
    public  class SearchCandidatesCommand
    {
        public string UserPrompt { get; init; } = string.Empty;

        public int MaxResults { get; init; } = 10;
    }
}
