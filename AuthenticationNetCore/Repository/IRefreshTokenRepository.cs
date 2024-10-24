namespace AuthenticationNetCore.Repository
{
    public interface IRefreshTokenRepository
    {
        void SaveRefreshToken(string userId, string refreshToken);
        string GetRefreshToken(string userId);
        void DeleteRefreshToken(string userId);
    }
}
