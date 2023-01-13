using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyDataCoinBridge.Entities;
using MyDataCoinBridge.Models;

namespace MyDataCoinBridge.Interfaces
{
    public interface IProviders
    {
        Task<DataProvider> GetProviderByIdAsync(Guid id);
        Task<DataProvider> GetProviderByToken(string token);
        Task<DataProvider> LogoUpload(string path, DataProvider provider);

        Task<List<ProvaidersRequest>> GetUsersProvidersAsync(string country, string userId);

        //Task<string> GetMashinaKgUserAsync(string email, string jwtToken);

        //<string> GetMashinaKgUserTransactionHistoryAsync(string email, string jwtToken);

        //Task<string> GetMashinaKgUserStatsAsync(string email, string jwtToken);

        Task<GeneralResponse> GetDataFromGoogleAsync(string jwtToken);

        Task<string> GetDataFromFacebookAsync(string jwtToken);

        // TODO: реализовать модель истории монетизации
        //public Task<MonetizationModel> GetTotalMonetizationStatAsync(string email);

        public Task<DataProviderRequest> POST(DataProviderRequest model);
        public Task<DataProviderRequest> GETBYID(string token);
        public Task<List<DataProviderRequest>> GETLIST();
        public Task<List<CountryRequest>> GETLISTCountry();
        public Task<DataProviderRequest> PUT(Guid id, DataProviderRequest model);
        public Task<DataProviderRequest> DELETE(Guid id);

        public Task<RewardCategoryRequest> RewardCategoryPOST(RewardCategoryRequest model);
        public Task<RewardCategoryRequest> RewardCategoryGETBYID(Guid id);
        public Task<List<RewardCategoryRequest>> RewardCategoryGETLIST();
        public Task<RewardCategoryRequest> RewardCategoryPUT(Guid id, RewardCategoryRequest model);
        public Task<RewardCategoryRequest> RewardCategoryDELETE(Guid id);

        public Task<TransactionRequest> TransactionPOST(TransactionRequest model);
        public Task<TransactionRequest> TransactionGETBYID(Guid id);
        public Task<List<TransactionRequest>> TransactionGETLIST();
        public Task<TransactionRequest> TransactionPUT(Guid id, TransactionRequest model);
        public Task<TransactionRequest> TransactionDELETE(Guid id);

        public Task<TermOfUse> TermOfUseStatus(Guid userId, Guid provaiderId, List<string> email, List<string> phone);
        public Task<TermOfUse> TermOfUseStatus(string userFIO, Guid userId, Guid provaiderId);
        public Task<bool> TermOfUseApply(Guid userId, Guid provaiderId);
        public Task<bool> TermOfUseCancel(Guid userId, Guid provaiderId);
        public Task<AllDataFromStatisticRequest> GetStatistics(string userId);
        public Task<AllDataFromStatisticRequest> GetStatisticsExtend(string userId);


        public Task<GeneralResponse> TransactionAddProvider(string token, List<TransactionProviderRequest> model);
        public Task<List<TransactionProviderResponse>> GetStatisticFromProvider(string token);
        public Task<List<TransactionProviderResponse>> GetStatisticFromProviderFromAdmin();
        public Task<decimal> GetClaimStatistic(string userId);
        public Task<GeneralResponse> Upload(Uploadrequest model);
    }
}

