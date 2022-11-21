using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyDataCoinBridge.Entities
{
	public class Country
	{
		public Country()
        {
            this.DataProviders = new HashSet<DataProvider>();
        }

		[Key]
        [JsonIgnore]
		public Guid Id { get; set; }

		/// <summary>
        /// Country name
        /// <example>Russia</example>
        /// </summary>
		[Required(ErrorMessage = "Country name can't be null")]
		public string CountryName { get; set; }

        /// <summary>
        /// Country code by ISO
        /// <example>RUS</example>
        /// </summary>
		[Required(ErrorMessage = "Country code can't be null")]
		public string CountryCode { get; set; }

		/// <summary>
        /// Country phone code by ISO
        /// <example>+996</example>
        /// </summary>
		public string PhoneCode { get; set; }

		public virtual ICollection<DataProvider> DataProviders { get; set; }
	}
}

