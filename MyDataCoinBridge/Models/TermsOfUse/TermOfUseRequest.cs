using MyDataCoinBridge.Entities;
using System;
using System.Collections.Generic;

namespace MyDataCoinBridge.Models.TermsOfUse
{
    public class TermOfUseRequest
    {
        public Guid userId { get; set; }
        public Guid provaiderId { get; set; }
        public CategoryTermsOfUse? CategoryTermsOfUse { get; set; } = Entities.CategoryTermsOfUse.Monetize;
        public List<string> email { get; set; }
        public List<string> phone { get; set; }
    }
}
