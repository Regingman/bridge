using MyDataCoinBridge.Models;
using System.Security.Claims;

namespace MyDataCoinBridge.Interfaces
{
    public interface IJWTManager
    {
        Tokens GenerateToken(string socialId);

        Tokens GenerateRefreshToken(string socialId);

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
