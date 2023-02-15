using System;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoinBridge.Models
{
    public class RewardCategoryByProviderRequest
    {
        [Key]
        public Guid Id { get; set; }
        public string RewardCategoryName { get; set; }
        public Guid RewardCategoryId { get; set; }
        public string Token { get; set; }
        public decimal Amount { get; set; }
    }
}
