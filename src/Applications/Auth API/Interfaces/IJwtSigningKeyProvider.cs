namespace Auth.API.Interfaces
{
    public interface IJwtSigningKeyProvider
    {
        string Key { get; }
    }
}
