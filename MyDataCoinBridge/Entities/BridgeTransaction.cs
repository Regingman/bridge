using System;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoinBridge.Entities
{
    public class BridgeTransaction
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string UserId { get; set; }
        public string ProviderId { get; set; }
        public string ProviderName { get; set; }
        public decimal Count { get; set; }
        public decimal USDMDC { get; set; }
        public decimal Commission { get; set; }
        public string RewardCategoryId { get; set; }
        public string RewardCategoryName { get; set; }
        public bool Claim { get; set; }
        public DateTime Created { get; set; }
    }
}
