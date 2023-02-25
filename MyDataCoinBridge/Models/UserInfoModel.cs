using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyDataCoinBridge.Models
{
    public class UserInfoModel
    {
        public int Action { get; set; }

        [JsonIgnore]
        public string OpenKey { get; set; }

        public string Server_Secret { get; set; }

        public Guid ProviderId { get; set; }

        public List<string> Emails { get; set; }

        public List<string> Phones { get; set; }

        [JsonIgnore]
        public int Video_VAST_desktop { get; set; }
        [JsonIgnore]
        public int Video_VAST_mobile { get; set; }
        [JsonIgnore]
        public int Multitag_mobile { get; set; }
        [JsonIgnore]
        public int In_page_desktop { get; set; }
        [JsonIgnore]
        public int Credit_History { get; set; }
        [JsonIgnore]
        public int Banner_mobile { get; set; }
        [JsonIgnore]
        public int Personal_Data { get; set; }
        [JsonIgnore]
        public int Banner_desktop { get; set; }
        [JsonIgnore]
        public int Conversion { get; set; }
        [JsonIgnore]
        public int Popunder_desktop { get; set; }
        [JsonIgnore]
        public int Multitag_desktop { get; set; }
        [JsonIgnore]
        public int View { get; set; }
        [JsonIgnore]
        public int Insurance_History { get; set; }
        [JsonIgnore]
        public int Popunder_mobile { get; set; }
        [JsonIgnore]
        public int Click { get; set; }
        [JsonIgnore]
        public int In_page_mobile { get; set; }
    }
}
