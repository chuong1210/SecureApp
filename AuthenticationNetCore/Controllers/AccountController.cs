using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationNetCore.Dtos;
using AuthenticationNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;


        public AccountController(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._configuration = configuration;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Register([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new AppUser
            {
                Email = registerDTO.Email,
                FullName = registerDTO.FullName,
                UserName = registerDTO.Email
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            if (registerDTO.Roles is null)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }
            else
            {
                foreach (var role in registerDTO.Roles)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            return Ok(new AuthResponseDTO
            {
                IsSuccess = true,
                Message = "Account created sucessfully"
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]

        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user is null)
            {
                return Unauthorized(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "user not found with this email"

                });
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!result)
            {
                return Unauthorized(new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Invalid passwordd"

                });
            }
            var token = generateToken(user);
            return Ok(new AuthResponseDTO
            {
                Token = token,
                IsSuccess = true,
                Message = "Account created sucessfully"
            }); ;
        }
        private string generateToken(AppUser user)
        {
            var tokenHandle = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("JWTSetting").GetSection("securityKey").Value!);
            var roles = _userManager.GetRolesAsync(user).Result;

            List<Claim> claims = new List<Claim>()
            {
                new (JwtRegisteredClaimNames.Email,user.Email??""),
                new (JwtRegisteredClaimNames.Name,user.FullName??""),
                new (JwtRegisteredClaimNames.NameId,user.Id??""),

                new (JwtRegisteredClaimNames.Aud,_configuration.GetSection("JWTSetting").GetSection("ValidAudicen").Value!),
                new (JwtRegisteredClaimNames.Iss,_configuration.GetSection("JWTSetting").GetSection("ValidIssuer").Value!),

            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
            };

            var token = tokenHandle.CreateToken(tokenDescriptor);
            return tokenHandle.WriteToken(token);

        }
        [HttpGet("detail")]
        public async Task<ActionResult<UserDetailDTO>> GetUserDetail()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user=await _userManager.FindByIdAsync(currentUserId);
            if ((user is null))
            {
                return NotFound(

                    new AuthResponseDTO
                    {
                        IsSuccess = false,
                        Message = "User not found"
                    }

                );

            }

            return new UserDetailDTO()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Roles = [.. await _userManager.GetRolesAsync(user)],
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                AccessFailedCount = user.AccessFailedCount,

            };
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UserDetailDTO>>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync(); // Lấy danh sách người dùng

            // Tạo danh sách UserDetailDTO
            var userDetails = new List<UserDetailDTO>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u); // Sử dụng await để lấy role không đồng bộ

                userDetails.Add(new UserDetailDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FullName,
                    Roles = roles.ToArray(), // Chuyển roles thành mảng
                    PhoneNumber = u.PhoneNumber
                });
            }

            return Ok(userDetails);
        }

        }


}
