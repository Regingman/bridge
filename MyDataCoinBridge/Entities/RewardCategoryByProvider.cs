using System;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoinBridge.Entities
{
    public class RewardCategoryByProvider
    {
        [Key]
        public Guid Id { get; set; }

        public RewardCategory RewardCategory { get; set; }
        public Guid RewardCategoryId { get; set; }

        public DataProvider DataProvider { get; set; }
        public Guid DataProviderId { get; set; }

        public decimal Amount { get; set; }
    }
}
