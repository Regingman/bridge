using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyDataCoinBridge.DataAccess;
using MyDataCoinBridge.Interfaces;

namespace MyDataCoinBridge.Services
{
	public class WebHooksService: IWebHooks
	{
        private readonly WebApiDbContext _context;
        private readonly ILogger<UserService> _logger;

        public WebHooksService(ILogger<UserService> logger, WebApiDbContext context)
		{
            _logger = logger;
            _context = context;
        }

        public async Task<string> Event(string message)
        {
            _logger.LogInformation($"Hello, {message}");
            return new string("sdfsdf");
        }

        public Task<bool> Subscribe(string token, string url)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Unsubscribe(string token)
        {
            throw new NotImplementedException();
        }
    }
}

