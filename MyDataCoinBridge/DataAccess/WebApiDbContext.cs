using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyDataCoinBridge.Entities;
using MyDataCoinBridge.Helpers;

namespace MyDataCoinBridge.DataAccess
{
    public class WebApiDbContext : DbContext
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<WebApiDbContext> _logger;

        public WebApiDbContext(IOptions<AppSettings> appSettings, ILogger<WebApiDbContext> logger)
        {
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(_appSettings.DB_CONNECTION);
        }


        public DbSet<Country> Countries { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<DataProvider> DataProviders { get; set; }

        public DbSet<RewardCategory> RewardCategories { get; set; }

    }
}
