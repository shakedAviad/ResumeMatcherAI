using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Commands
{
    public  class IngestResumesCommand
    {
        public IReadOnlyList<string> FilePaths { get; init; } = [];
    }
}
