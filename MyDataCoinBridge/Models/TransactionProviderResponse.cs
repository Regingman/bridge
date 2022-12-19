using System;

namespace MyDataCoinBridge.Models
{
    public class TransactionProviderResponse
    {
        public string EmailPhone { get; set; }
        public string RewardCategoryName { get; set; }
        public decimal Count { get; set; }
        public decimal USDMCDAmount { get; set; }
        public DateTime Created { get; set; }
    }
}
