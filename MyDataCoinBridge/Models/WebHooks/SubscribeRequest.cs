using System;
namespace MyDataCoinBridge.Models.WebHooks
{
	public class SubscribeRequest
	{
		public string Token { get; set; }

		public string URL { get; set; }
	}
}

