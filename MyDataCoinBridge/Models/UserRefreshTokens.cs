namespace MyDataCoinBridge.Models
{
    public class UserRefreshTokenResponse
    {
        public string Email { get; set; }
        public string RefreshToken { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
