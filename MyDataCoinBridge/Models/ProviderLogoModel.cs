using Microsoft.AspNetCore.Http;

namespace MyDataCoinBridge.Models
{
    public class ProviderLogoModel
    {
        public IFormFile Logo { get; set; }
        public string Token { get; set; }
    }
}
