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
}
