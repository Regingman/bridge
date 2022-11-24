using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyDataCoinBridge.Entities
{
    public class UserTermsOfUse
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public Guid DataProviderId { get; set; }
        public DataProvider? DataProvider { get; set; }
        public bool IsRegistered { get; set; } = false;
    }
}
