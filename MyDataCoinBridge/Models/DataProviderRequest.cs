﻿using MyDataCoinBridge.Entities;
using System;
using System.Collections.Generic;

namespace MyDataCoinBridge.Models
{
    public class DataProviderRequest
    {
        public DataProviderRequest()
        {
            this.RewardCategories = new HashSet<RewardCategoryRequest>();
            this.Countries = new HashSet<CountryRequest>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string BridgeUserId { get; set; }

        public string Token { get; set; }
        
        public bool Connected { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<RewardCategoryRequest> RewardCategories { get;  set; }

        public ICollection<CountryRequest> Countries { get; set; }

        public string VerificationCode { get; set; }

        public VerifiedEnum IsVerified { get; set; }

        public string BridgeUserEmail { get; set; }
    }
}
