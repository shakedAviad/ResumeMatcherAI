namespace Core.Commands
{
    public class SearchCandidatesCommand : BaseUserInputCommand
    {
        public int MaxResults { get; init; } = 10;
    }
}
