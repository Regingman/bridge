using System;
using MyDataCoinBridge.Enums;

namespace MyDataCoinBridge.Models.WebHooks
{
	public class WebHookResponse
	{
		public HookEventType Action  { get; set; }

		public string Payload { get; set; }
	}
}

