namespace AuthenticationNetCore.Dtos
{
    public class TokenRequest
    {
        public string RefreshToken { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
