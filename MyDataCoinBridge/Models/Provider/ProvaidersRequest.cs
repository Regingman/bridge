using MyDataCoinBridge.Entities;
using System.Collections.Generic;

namespace MyDataCoinBridge.Models.Provider
{
    public class ProvaidersRequest
    {
        public ProvaidersRequest()
        {
            this.DataProviders = new HashSet<DataProviderRequest>();
        }

        /// <summary>
        /// Country name
        /// <example>Russia</example>
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Country code by ISO
        /// <example>RUS</example>
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Country phone code by ISO
        /// <example>+996</example>
        /// </summary>
        public string PhoneCode { get; set; }

        public virtual ICollection<DataProviderRequest> DataProviders { get; set; }
    }
}
