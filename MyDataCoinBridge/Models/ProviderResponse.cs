using System;
using System.Collections.Generic;
using MyDataCoinBridge.Entities;

namespace MyDataCoinBridge.Models
{
    public class ProviderResponse
    {
        public DataProvider dataProvider { get; set; }

        public List<RewardCategory> RewardsCategories { get; set; }
    }
}

