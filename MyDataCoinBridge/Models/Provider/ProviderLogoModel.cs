using Microsoft.AspNetCore.Http;

namespace MyDataCoinBridge.Models.Provider
{
    public class ProviderLogoModel
    {
        public IFormFile Logo { get; set; }
        public string Token { get; set; }
    }
}
