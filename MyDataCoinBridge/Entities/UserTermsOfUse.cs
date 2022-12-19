using System;
using System.Collections.Generic;
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
        public List<string> Email { get; set; } = new List<string>();
        public List<string> Phone { get; set; } = new List<string>();
    }
}
