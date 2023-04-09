
using System.Collections.Generic;
using MyDataCoinBridge.Models.Provider;

namespace MyDataCoin.Models
{
    public class ProvidersListResponse
    {
        public ProvidersListResponse(int code, string errorMessage)
        {
            Code = code;
            ErrorMessage = errorMessage;
        }
        public ProvidersListResponse(int code, string errorMessage, List<ProvidersRequest> providers, bool claimable)
        {
            Code = code;
            ErrorMessage = errorMessage;
            Providers = providers;
            Claimable = claimable;
        }

        public int Code { get; set; }

        public string ErrorMessage { get; set; }

        public List<ProvidersRequest> Providers { get; set; }
        public bool Claimable { get; set; }
    }
}

