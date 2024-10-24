namespace AuthenticationNetCore.Repository
{
    public interface IInvalidatedTokenRepository
    {
        Task InvalidateTokenAsync(string token, string userid);
        Task<bool> IsTokenInvalidated(string token);
    }
}
