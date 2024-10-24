using AuthenticationNetCore.Repository;
using AuthenticationNetCore.Service;
using Microsoft.AspNetCore.Identity.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using AuthenticationNetCore.Dtos;
using AuthenticationNetCore.Repository.imp;
using Microsoft.EntityFrameworkCore;
using AuthenticationNetCore.Models;

namespace AuthenticationNetCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IInvalidatedTokenRepository _invalidatedTokenService;
        public UserController(TokenService tokenService, IRefreshTokenRepository refreshTokenRepository, IInvalidatedTokenRepository invalidatedTokenService)
        {
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _invalidatedTokenService = invalidatedTokenService;

        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Giả sử user đã được xác thực qua cơ chế nào đó (có thể là từ DB)
            var userId = "user_id"; // Đây là userId sau khi đã xác thực

            var accessToken = _tokenService.GenerateAccessToken(userId);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Lưu refresh token vào repository
            _refreshTokenRepository.SaveRefreshToken(userId, refreshToken);

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenRequest request)
        {
            //var invalidatedToken = await _context.InvalidatedTokens
            //    .FirstOrDefaultAsync(t => t.Token == request.AccessToken);
            var check = await _invalidatedTokenService.IsTokenInvalidated(request.AccessToken); // phải có await

            if (check)
            {
                return BadRequest("Token has been invalidated");
            }

            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                return BadRequest("Invalid access token");
            }

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var savedRefreshToken = _refreshTokenRepository.GetRefreshToken(userId);

            if (savedRefreshToken != request.RefreshToken)
            {
                return BadRequest("Invalid refresh token");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(userId);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            // Cập nhật refresh token mới
            _refreshTokenRepository.SaveRefreshToken(userId, newRefreshToken);

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("your_secret_key_here")),
                ValidIssuer = "your_issuer",
                ValidAudience = "your_audience",
                ValidateLifetime = false  // Chỉ validate JWT mà không cần kiểm tra thời gian sống (expired)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] TokenRequest request)
        {
            // Lưu token bị thu hồi vào cơ sở dữ liệu
            await _invalidatedTokenService.InvalidateTokenAsync(request.AccessToken, User.FindFirstValue(ClaimTypes.NameIdentifier));


            //var invalidatedToken = new InvalidatedToken
            //{
            //    Token = request.Token,
            //    InvalidatedAt = DateTime.UtcNow,
            //    UserId = request.UserId  // Lưu thông tin người dùng liên quan đến token này
            //};

            //_context.InvalidatedTokens.Add(invalidatedToken);
            //await _context.SaveChangesAsync();
            return Ok(new { message = "User logged out and token invalidated" });
        }
    }
}
