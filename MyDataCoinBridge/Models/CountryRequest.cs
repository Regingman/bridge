using System;

namespace MyDataCoinBridge.Models
{
    public class CountryRequest
	{
		public Guid Id { get; set; }
		public string CountryName { get; set; }
		public string CountryCode { get; set; }
		public string PhoneCode { get; set; }
	}
}
