using AuthenticationNetCore.Dtos;
using AuthenticationNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationNetCore.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin")]
  //  [QuyenAuthorize(Quyen = "Admin")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleController(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDTO createRoleDTO)
        {
            if (string.IsNullOrEmpty(createRoleDTO.RoleName))
            {
                return BadRequest("Role name is required");
            }

            var roleExist= await _roleManager.RoleExistsAsync(createRoleDTO.RoleName);
            if (roleExist) {
                return BadRequest("Role already exist");
            }

            var roleResult = await _roleManager.CreateAsync(new IdentityRole(createRoleDTO.RoleName));
            if (roleResult.Succeeded)
            {

                return Ok(new {message="Role created sucessfully"});
            }

            return BadRequest("Role creation failed");

        }

        [HttpGet]
        public async  Task<ActionResult<IEnumerable<RoleResponseDTO>>> getRoles()
        {
            //var roles = await _roleManager.Roles.Select(role =>
            //    new RoleResponseDTO
            //    {
            //        Id = role.Id,
            //        Name = role.Name,
            //        TotalUsers = _userManager.GetUsersInRoleAsync(role.Name!).Result.Count,
            //    }).ToListAsync();

            var roles =  _roleManager.Roles.ToList();
            return  Ok(roles);
        }
        [AllowAnonymous]

        [HttpGet("roles-with-user-count")]
        public async Task<IActionResult> GetRolesWithUserCount()
        {
            var roles = _roleManager.Roles.ToList();
            var rolesWithUserCount = new List<RoleResponseDTO>();

            foreach (var role in roles)
            {
                // Đếm số lượng người dùng trong vai trò này
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                rolesWithUserCount.Add(new RoleResponseDTO
                {
                    Name = role.Name,
                    TotalUsers = usersInRole.Count
                });
            }

            return Ok(rolesWithUserCount);
        }

    [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role= await _roleManager.FindByIdAsync(id);
            if(role == null)
            {
                return NotFound("Role not found");
            }
             var result=await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return Ok(new { message = "Role deleted sucessfully" });
            }
            return BadRequest("Role deletion failed");

        }


        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            var user = await _userManager.FindByIdAsync(assignRoleDto.UserId);
            if (user is null)
            {
                return NotFound("User not found");

            }

            var role = await _roleManager.FindByIdAsync(assignRoleDto.RoleId);
            if (role is null)
            {
                return NotFound("Role not found");
            }


            var result = await _userManager.AddToRoleAsync(user, role.Name!);

            if(result.Succeeded)
            {
                return Ok(new { message = "Created role successfully" });
            }

            var error=result.Errors.FirstOrDefault();
            return BadRequest(error!.Description);
        }
    }
}
