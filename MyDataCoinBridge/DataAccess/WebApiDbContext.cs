using Microsoft.EntityFrameworkCore;
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
        public DbSet<RewardCategoryByProvider> RewardCategoryByProviders { get; set; }
        public DbSet<RewardCategory> RewardCategories { get; set; }
        public DbSet<UserTermsOfUse> UserTermsOfUses { get; set; }
        public DbSet<BridgeUser> BridgeUsers { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<BridgeTransaction> BridgeTransactions { get; set; }

        public DbSet<UserPrivacySetting> UserPrivacySettings { get; set; }

        public DbSet<WebHook> WebHooks { get; set; }
        public DbSet<WebHookRecord> WebHooksHistory { get; set; }
    }
}
