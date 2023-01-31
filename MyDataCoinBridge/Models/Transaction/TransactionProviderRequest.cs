namespace MyDataCoinBridge.Models.Transaction
{
    public class TransactionProviderRequest
    {
        public string EmailPhone { get; set; }
        public string RewardCategoryId { get; set; }
        public decimal Count { get; set; }
    }
}
