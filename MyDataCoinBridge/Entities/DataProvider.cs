using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyDataCoinBridge.Entities
{
    /// <summary>
    /// It is assumed that if one provider is present in more than one country,
    /// then another provider should be created for each country.
    /// </summary>
    public class DataProvider
    {
        public DataProvider()
        {
            this.RewardCategories = new HashSet<RewardCategory>();
            this.Countries = new HashSet<Country>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public BridgeUser BridgeUser { get; set; }

        public Guid? BridgeUserId { get; set; }

        public string Icon { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<RewardCategory> RewardCategories { get; set; }

        public ICollection<Country> Countries { get; set; }
    }
}
