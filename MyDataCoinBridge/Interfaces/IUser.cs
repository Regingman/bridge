using MyDataCoinBridge.Models;
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
    }
}
