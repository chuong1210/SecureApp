using AuthenticationNetCore.Data;
using AuthenticationNetCore.Models;
using Azure.Core;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationNetCore.Repository.imp
{

    public class InvalidatedTokenRepository : IInvalidatedTokenRepository
    {
        private readonly AppDBContext _context;

        public InvalidatedTokenRepository(AppDBContext context)
        {
            _context = context;
        }

        // Lưu token bị thu hồi vào cơ sở dữ liệu
        public async Task InvalidateTokenAsync(string token, string userId)
        {
            var invalidatedToken = new InvalidatedToken
            {
                Token = token,
                InvalidatedAt = DateTime.UtcNow,
                UserId = userId
            };

            _context.InvalidatedTokens.Add(invalidatedToken);
            await _context.SaveChangesAsync();
        }

        // Kiểm tra xem token đã bị thu hồi chưa
        public async Task<bool> IsTokenInvalidated(string token)
        {
            return await _context.InvalidatedTokens
                .AnyAsync(t => t.Token == token);
        }
        public async Task<string> CheckTokenRevoke(string token)
        {
            var invalidatedToken = await _context.InvalidatedTokens
                .FirstOrDefaultAsync(t => t.Token == token);

            if (invalidatedToken != null)
            {
                return "Token has been revoked";
            }
            else
            {
                return "Token is valid";
            }
        }


    }
}
