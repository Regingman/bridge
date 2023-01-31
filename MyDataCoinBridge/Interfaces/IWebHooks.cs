using System;
using System.Threading.Tasks;

namespace MyDataCoinBridge.Interfaces
{
	public interface IWebHooks
	{
		Task<bool> Subscribe(string token, string url);

		Task<bool> Unsubscribe(string token);

		Task<string> Event(string message);
	}
}

