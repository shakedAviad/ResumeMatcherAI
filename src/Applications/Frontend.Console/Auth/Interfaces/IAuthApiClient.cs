namespace Frontend.Console.Auth.Interfaces
{
    public interface IAuthApiClient
    {
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
        string AcessToken { get; }
    }
}
