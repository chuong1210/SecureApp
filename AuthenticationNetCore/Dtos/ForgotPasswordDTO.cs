using System.ComponentModel.DataAnnotations;

namespace AuthenticationNetCore.Dtos
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
