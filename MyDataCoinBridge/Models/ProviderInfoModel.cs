using Newtonsoft.Json;
using System.Collections.Generic;

namespace MyDataCoinBridge.Models
{
    public class ProviderInfoModel
    {
        public string Secret { get; set; }
        public int Action { get; set; }
        public List<string> Email { get; set; }
        public List<string> Phone { get; set; }
    }

    public class ProviderInfoModelProvicy
    {
        public int Action { get; set; }
        public string Secret { get; set; }
        public int Video_VAST_desktop { get; set; }
        public int Video_VAST_mobile { get; set; }
        public int Multitag_mobile { get; set; }
        public int In_page_desktop { get; set; }
        public int Credit_History { get; set; }
        public int Banner_mobile { get; set; }
        public int Personal_Data { get; set; }
        public int Banner_desktop { get; set; }
        public int Conversion { get; set; }
        public int Popunder_desktop { get; set; }
        public int Multitag_desktop { get; set; }
        public int View { get; set; }
        public int Insurance_History { get; set; }
        public int Popunder_mobile { get; set; }
        public int Click { get; set; }
        public int In_page_mobile { get; set; }
    }
}
