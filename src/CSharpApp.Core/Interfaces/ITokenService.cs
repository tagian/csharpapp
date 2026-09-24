public interface ITokenService
{
    Task<string> GetAccessTokenAsync(CancellationToken ct);
}