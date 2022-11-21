using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace MyDataCoinBridge.Models
{
    public class Client
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }

    public class ClientInfo
    {
        public Client Client { get; set; }

        private readonly IConfiguration _configuration;

        public ClientInfo(IConfiguration configuration)
        {
            _configuration = configuration;
            Client = Load();
        }

        private Client Load()
        {

            var configuration = _configuration.GetSection("Authentication:Google");
            return new Client
            {
                client_id = configuration["client_id"],
                client_secret = configuration["client_secret"],
            };
        }
    }
}
