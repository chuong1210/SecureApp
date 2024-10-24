namespace AuthenticationNetCore.Dtos
{
    public class AuthResponseDTO
    {
        public string? Token { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string refreshToken { get; set; }=string.Empty;
    }
}
