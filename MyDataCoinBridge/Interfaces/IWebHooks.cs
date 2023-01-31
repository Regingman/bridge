using System;
using System.Threading.Tasks;
using MyDataCoinBridge.Models;
using MyDataCoinBridge.Models.WebHooks;

namespace MyDataCoinBridge.Interfaces
{
	public interface IWebHooks
	{
		Task<GeneralResponse> Subscribe(SubscribeRequest model);

		Task<GeneralResponse> Unsubscribe(string token);

		Task<string> Event(WebHookUserProfileModel.Profile message);
	}
}

