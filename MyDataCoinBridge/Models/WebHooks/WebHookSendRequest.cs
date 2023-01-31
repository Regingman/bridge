using System;
using MyDataCoinBridge.Enums;

namespace MyDataCoinBridge.Models.WebHooks
{
	public class WebHookSendRequest
	{
		public string Secret { get; set; }
		public HookEventType Action { get; set; }
		public string[] Email { get; set; }
		public string[] Phone { get; set; }
	}
}

