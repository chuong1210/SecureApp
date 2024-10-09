using System.ComponentModel.DataAnnotations;

namespace AuthenticationNetCore.Dtos
{
    public class CreateRoleDTO
    {
        [Required(ErrorMessage = "Role Name is required.")]
        public string RoleName { get; set; } = null!;
    }
}
