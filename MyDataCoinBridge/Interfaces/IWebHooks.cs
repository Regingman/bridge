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

		Task<GeneralResponse> GetUrl(string token);

		Task<GeneralResponse> EditUrl(SubscribeRequest model);
	}
}

