using System.Collections.Generic;
using AuthenticationNetCore.Repository;

public class InMemoryRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly Dictionary<string, string> _refreshTokens = new Dictionary<string, string>();

    public void SaveRefreshToken(string userId, string refreshToken)
    {
        _refreshTokens[userId] = refreshToken;
    }

    public string GetRefreshToken(string userId)
    {
        _refreshTokens.TryGetValue(userId, out var refreshToken);
        return refreshToken;
    }

    public void DeleteRefreshToken(string userId)
    {
        _refreshTokens.Remove(userId);
    }
}
