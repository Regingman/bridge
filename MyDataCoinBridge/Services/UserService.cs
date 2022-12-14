using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyDataCoinBridge.DataAccess;
using MyDataCoinBridge.Helpers;
using MyDataCoinBridge.Interfaces;
using MyDataCoinBridge.Models;
using System;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace MyDataCoinBridge.Services
{
    public class UserService : IUser
    {
        private readonly WebApiDbContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly IJWTManager _jWTManager;

        public UserService(ILogger<UserService> logger, WebApiDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<GeneralResponse> SendCode(string email)
        {

            try
            {
                var user = await _context.BridgeUsers.SingleOrDefaultAsync(x => x.Email == email.ToLower());

                if (user == null)
                    return new GeneralResponse(400, "User not found");
                else
                {
                    string code = StaticFunctions.GenerateCode();
                    StaticFunctions.SendCode(email, code);
                    user.VerificationCode = BC.HashPassword(code);
                    await _context.SaveChangesAsync();
                    return new GeneralResponse(200, "Ok");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResponse(400, ex.Message);
            }
        }

        public async Task<VerifyCodeResponse> VerifyCode(VerifyCodeRequest request)
        {
            var user = await _context.BridgeUsers.SingleOrDefaultAsync(x => x.Email == request.Email);

            if (user == null || !BC.Verify(request.Code, user.VerificationCode))
            {
                return new VerifyCodeResponse(null, 400, "User not found or incorrect verification code");
            }
            else
            {
                var token = _jWTManager.GenerateToken(request.Email);
                if (token == null)
                {
                    return new VerifyCodeResponse(null, 400, "Invalid Token");
                }

                // saving refresh token to the db
                UserRefreshTokenResponse obj = new UserRefreshTokenResponse
                {
                    RefreshToken = token.Refresh_Token,
                    Email = request.Email
                };

                //AddUserRefreshTokens(obj);
                await _context.SaveChangesAsync();

                return new VerifyCodeResponse(token, 200, "Success");
            }
        }
    }
}
