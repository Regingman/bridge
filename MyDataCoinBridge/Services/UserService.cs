using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyDataCoinBridge.DataAccess;
using MyDataCoinBridge.Entities;
using MyDataCoinBridge.Helpers;
using MyDataCoinBridge.Interfaces;
using MyDataCoinBridge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace MyDataCoinBridge.Services
{
    public class UserService : IUser
    {
        //private readonly AppSettings _appSettings;
        private readonly WebApiDbContext _context;
        private readonly ILogger<UserService> _logger;
        private readonly IJWTManager _jWTManager;

        public UserService(ILogger<UserService> logger, WebApiDbContext context, IJWTManager jWTManager/*, AppSettings appSettings*/)
        {
            _logger = logger;
            _context = context;
            _jWTManager = jWTManager;
            //_appSettings = appSettings;
        }

        public async Task<GeneralResponse> Registration(string email)
        {
            try
            {

                var user = await _context.BridgeUsers.SingleOrDefaultAsync(x => x.Email == email.ToLower());

                if (user != null)
                    return new GeneralResponse(400, "This Email used! Pls try again!");
                else
                {
                    BridgeUser bridgeUser = new BridgeUser();
                    bridgeUser.Email = email.ToLower();
                    bridgeUser.CreatedAt = DateTime.UtcNow;
                    bridgeUser.Role = Roles.User;
                    bridgeUser.TokenForService = BC.HashPassword(StaticFunctions.GenerateCode());
                    bridgeUser.IsVerified = VerifiedEnum.No;

                    string code = StaticFunctions.GenerateCode();
                    StaticFunctions.SendCode(email, code, "123123qQq");
                    bridgeUser.VerificationCode = BC.HashPassword(code);
                    await _context.BridgeUsers.AddAsync(bridgeUser);
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
                    StaticFunctions.SendCode(email, code, "123123qQq");
                    user.VerificationCode = BC.HashPassword(code);
                    await _context.SaveChangesAsync();
                    return new GeneralResponse(200, "OK");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResponse(400, ex.Message);
            }
        }

        public async Task<GeneralResponse> Verify(string email)
        {
            try
            {
                var user = await _context.BridgeUsers.FirstOrDefaultAsync(x => x.Email == email.ToLower());

                if (user == null)
                    return new GeneralResponse(400, "User not found! Pls try again!");
                else
                {
                    user.IsVerified = VerifiedEnum.Yes;
                    _context.BridgeUsers.Update(user);
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

        public async Task<GeneralResponse> VerifyRequest(string token)
        {
            try
            {
                var user = await _context.BridgeUsers.FirstOrDefaultAsync(x => x.TokenForService == token);

                if (user == null)
                    return new GeneralResponse(400, "User not found! Pls try again!");
                else
                {
                    user.IsVerified = VerifiedEnum.Await;
                    _context.BridgeUsers.Update(user);
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
                UserRefreshToken obj = new UserRefreshToken
                {
                    RefreshToken = token.Refresh_Token,
                    Email = request.Email
                };

                AddUserRefreshTokens(obj);
                await _context.SaveChangesAsync();

                return new VerifyCodeResponse(token, 200, "Success", token: user.TokenForService, isVerified: user.IsVerified, role: user.Role.ToString(), email: request.Email);
            }
        }

        public async Task<RefreshResponse> Refresh(Tokens tokens)
        {
            try
            {
                var principal = _jWTManager.GetPrincipalFromExpiredToken(tokens.Access_Token);
                if (principal == null)
                {
                    return new RefreshResponse(401, "Token LifeTime ended!");
                }
                var socialId = principal.Identity?.Name;

                //retrieve the saved refresh token from database
                var savedRefreshToken = GetSavedRefreshTokens(socialId, tokens.Refresh_Token);
                if (savedRefreshToken == null)
                {
                    return new RefreshResponse(401, "Invalid attempt!");
                }
                if (savedRefreshToken.RefreshToken != tokens.Refresh_Token)
                {
                    return new RefreshResponse(401, "Invalid attempt!");
                }

                var newJwtToken = _jWTManager.GenerateRefreshToken(socialId);

                if (newJwtToken == null)
                {
                    return new RefreshResponse(401, "Invalid attempt!");
                }

                // saving refresh token to the db
                UserRefreshToken obj = new UserRefreshToken
                {
                    RefreshToken = newJwtToken.Refresh_Token,
                    Email = socialId,
                };

                DeleteUserRefreshTokens(socialId, tokens.Refresh_Token);
                AddUserRefreshTokens(obj);
                await _context.SaveChangesAsync();

                return new RefreshResponse(200, newJwtToken);
            }
            catch (Exception ex)
            {
                return new RefreshResponse(401, "Invalid attempt!");
            }
        }

        public UserRefreshToken AddUserRefreshTokens(UserRefreshToken user)
        {
            _context.UserRefreshTokens.Add(user);
            return user;
        }

        public void DeleteUserRefreshTokens(string socialId, string refreshToken)
        {
            UserRefreshToken item = _context.UserRefreshTokens.FirstOrDefault(x => x.Email == socialId && x.RefreshToken == refreshToken);
            if (item != null)
            {
                _context.UserRefreshTokens.Remove(item);
            }
        }

        public UserRefreshToken GetSavedRefreshTokens(string socialId, string refreshToken)
        {
            return _context.UserRefreshTokens.FirstOrDefault(x => x.Email == socialId && x.RefreshToken == refreshToken && x.IsActive == true);
        }

        public async Task<List<BridgeUser>> UserList()
        {
            return await _context.BridgeUsers.ToListAsync();
        }

        public async Task<GeneralResponse> SetManager(string userId)
        {
            try
            {
                var user = await _context.BridgeUsers.FirstOrDefaultAsync(e => e.Id == Guid.Parse(userId));
                user.Role = Roles.Manager;
                _context.BridgeUsers.Update(user);
                await _context.SaveChangesAsync();
                return new GeneralResponse(200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResponse(400, ex.Message);
            }
        }
    }
}
