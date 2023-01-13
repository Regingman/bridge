using MyDataCoinBridge.Entities;
using MyDataCoinBridge.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyDataCoinBridge.Interfaces
{
    public interface IUser
    {
        /// <summary>
        /// Registration
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<GeneralResponse> Registration(string email);
        /// <summary>
        /// Refresh token
        /// </summary>
        public Task<RefreshResponse> Refresh(Tokens tokens);
        /// <summary>
        /// Verify user provider
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<GeneralResponse> Verify(string email);
        /// <summary>
        /// Verify request user provider
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<GeneralResponse> VerifyRequest(string token);
        /// <summary>
        /// Send authorization code for user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public Task<GeneralResponse> SendCode(string email);
        /// <summary>
        /// Verify authorization code
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        public Task<VerifyCodeResponse> VerifyCode(VerifyCodeRequest request);

        /// <summary>
        /// User list
        /// </summary>
        public Task<List<BridgeUser>> UserList();

        /// <summary>
        /// User list
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<GeneralResponse> SetManager(string userId);
    }
}
