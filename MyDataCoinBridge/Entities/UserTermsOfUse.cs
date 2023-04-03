using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoinBridge.Entities
{
    public class UserTermsOfUse
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public Guid DataProviderId { get; set; }
        public CategoryTermsOfUse CategoryTermsOfUse { get; set; } = CategoryTermsOfUse.Monetize;
        #nullable enable
        public DataProvider? DataProvider { get; set; }
        #nullable disable

        public bool IsRegistered { get; set; } = false;
        public List<string> Email { get; set; } = new List<string>();
        public List<string> Phone { get; set; } = new List<string>();
    }

    public enum CategoryTermsOfUse
    {
        Monetize = 3,
        Sbor = 1,
        Obrabotka = 2
    }
}
