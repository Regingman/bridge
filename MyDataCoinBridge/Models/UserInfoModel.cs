using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyDataCoinBridge.Models
{
    public class UserInfoModel
    {
        public int Action { get; set; }

        public string Server_Secret { get; set; }

        public Guid ProviderId { get; set; }

        public List<string> Emails { get; set; }

        public List<string> Phones { get; set; }
    }
}
