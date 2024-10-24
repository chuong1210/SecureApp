namespace AuthenticationNetCore.Models
{
    public class InvalidatedToken
    {
        public int Id { get; set; }
        public string Token { get; set; }       // Token lưu trữ dưới dạng chuỗi
        public DateTime InvalidatedAt { get; set; }  // Thời gian bị thu hồi
        public string UserId { get; set; }      // User liên quan đến token bị thu hồi
    }
}
