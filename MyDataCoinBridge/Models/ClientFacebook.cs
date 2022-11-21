using Microsoft.Extensions.Configuration;

namespace MyDataCoinBridge.Models
{
    public class ClientFacebook
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }

    public class ClientInfoFacebook
    {
        public ClientFacebook Client { get; set; }

        private readonly IConfiguration _configuration;

        public ClientInfoFacebook(IConfiguration configuration)
        {
            _configuration = configuration;
            Client = Load();
        }

        private ClientFacebook Load()
        {

            var configuration = _configuration.GetSection("Authentication:FB");
            return new ClientFacebook
            {
                client_id = configuration["client_id"],
                client_secret = configuration["client_secret"],
            };
        }
    }
}
