using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyDataCoinBridge.Entities
{
    public class RewardCategory
    {
        public RewardCategory()
        {
            this.DataProviders = new HashSet<DataProvider>();
        }

        /// <summary>
        /// Unique identifier of reward category
        /// <example>e12318b4-dbc7-4d18-87c2-100bed209dce</example>
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Enum of reward category
        /// <example>Click</example>
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Description of reward category
        /// <example>Reward description</example>
        /// </summary>
        public string Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<DataProvider> DataProviders { get; set; }
    }
}

