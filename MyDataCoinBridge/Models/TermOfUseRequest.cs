using System;
using System.Collections.Generic;

namespace MyDataCoinBridge.Models
{
    public class TermOfUseRequest
    {
        public Guid userId { get; set; }
        public Guid provaiderId { get; set; }
        public List<string> email { get; set; }
        public List<string> phone { get; set; }
    }
}
