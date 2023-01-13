using MyDataCoinBridge.Entities;

namespace MyDataCoinBridge.Models
{
    public class Tokens
    {
        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
    }

    public class VerifyCodeResponse
    {
        public VerifyCodeResponse(Tokens tokens, int code, string errorMessage, string token, VerifiedEnum isVerified, string role, string email)
        {
            Tokens = tokens;
            Code = code;
            Message = errorMessage;
            Token = token;
            IsVerified = isVerified;
            Role = role;
            Email = email;

        }
        public VerifyCodeResponse(Tokens tokens, int code, string errorMessage)
        {
            Tokens = tokens;
            Code = code;
            Message = errorMessage;
        }

        public Tokens Tokens { get; set; }

        public int Code { get; set; }

        public string Message { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public VerifiedEnum IsVerified { get; set; }
    }
}
