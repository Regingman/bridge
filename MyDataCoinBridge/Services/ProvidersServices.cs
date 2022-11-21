using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyDataCoinBridge.DataAccess;
using MyDataCoinBridge.Entities;
using MyDataCoinBridge.Helpers;
using MyDataCoinBridge.Interfaces;
using MyDataCoinBridge.Models;
using Newtonsoft.Json.Linq;

namespace MyDataCoinBridge.Services
{
    public class ProvidersServices : IProviders
    {
        private readonly WebApiDbContext _context;
        private readonly IHttpClientFactory _clientFactory;
        private readonly AppSettings _appSettings;

        public ProvidersServices(
            IHttpClientFactory clientFactory,
            IOptions<AppSettings> appSettings,
            WebApiDbContext context)
        {
            _appSettings = appSettings.Value;
            _clientFactory = clientFactory;
            _context = context;
        }

        public async Task<DataProvider> GetProviderByIdAsync(Guid id)
        {
            return await _context.DataProviders.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Country>> GetUsersProvidersAsync(string countryCode)
        {
            return await _context.Countries
                .Include(provider => provider.DataProviders)
                .Where(x => x.CountryCode == countryCode)
                .ToListAsync();
        }

        public async Task<string> GetDataFromFacebookAsync(string jwtToken)
        {
            try
            {
                var uri = _appSettings.BRIDGE_URI;
                var request = new HttpRequestMessage(HttpMethod.Get, uri +
                        "api/AuthorizeControllers/GetPersonInfoFB");
                var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception e)
                    {
                        return e.Message;
                    }
                }
                else
                {
                    return response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<GeneralResponse> GetDataFromGoogleAsync(string jwtToken)
        {
            try
            {
                var uri = _appSettings.BRIDGE_URI;
                var request = new HttpRequestMessage(HttpMethod.Get, uri +
                        "api/AuthorizeControllers/GetPersonInfoGoogle?accessToken=" + jwtToken);
                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var myCleanJsonObject = JObject.Parse(result);
                        return new GeneralResponse(200, myCleanJsonObject);
                    }
                    catch (Exception e)
                    {
                        return new GeneralResponse(400, e.Message);
                    }
                }
                else
                {
                    return new GeneralResponse(200, response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                return new GeneralResponse(400, ex.Message);
            }
        }

        public async Task<string> GetMashinaKgUserAsync(string email, string jwtToken)
        {
            try
            {
                var uri = _appSettings.BRIDGE_URI;
                var request = new HttpRequestMessage(HttpMethod.Get, uri +
                        "api/AuthorizeControllers/GetPersonInfoMashinaKG");
                var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception e)
                    {
                        return e.Message;
                    }
                }
                else
                {
                    return response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> GetMashinaKgUserStatsAsync(string email, string jwtToken)
        {
            try
            {
                var uri = _appSettings.BRIDGE_URI;
                var request = new HttpRequestMessage(HttpMethod.Get, uri +
                        "api/AuthorizeControllers/GetStatsInfoMashinaKG");
                var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception e)
                    {
                        return e.Message;
                    }
                }
                else
                {
                    return response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> GetMashinaKgUserTransactionHistoryAsync(string email, string jwtToken)
        {
            try
            {
                var uri = _appSettings.BRIDGE_URI;
                var request = new HttpRequestMessage(HttpMethod.Get, uri +
                        "api/AuthorizeControllers/GetTransactionMashinaKG");
                var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + jwtToken);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception e)
                    {
                        return e.Message;
                    }
                }
                else
                {
                    return response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //public Task<MonetizationModel> GetTotalMonetizationStatAsync(string email)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<DataProviderRequest> POST(DataProviderRequest model)
        {
            var dataProvider = await _context.DataProviders.FirstOrDefaultAsync(e => e.Email == model.Email);
            {
                if (dataProvider == null)
                {
                    dataProvider = new DataProvider()
                    {
                        Address = model.Address,
                        CreatedAt = model.CreatedAt,
                        Countries = model.Countries.Select(e => new Country()
                        {
                            CountryCode = e.CountryCode,
                            CountryName = e.CountryName,
                            PhoneCode = e.PhoneCode
                        }).ToList(),
                        Email = model.Email,
                        Icon = model.Icon,
                        Name = model.Name,
                        Phone = model.Phone,
                        RewardCategories = model.RewardCategories.Select(e => new RewardCategory()
                        {
                            Description = e.Description,
                            Name = e.Name
                        }).ToList(),
                    };
                    await _context.DataProviders.AddAsync(dataProvider);
                    await _context.SaveChangesAsync();
                    model.Id = dataProvider.Id;
                    return model;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<DataProviderRequest> GETBYID(Guid id) => await _context.DataProviders
            .Select(e => new DataProviderRequest()
            {
                Id = e.Id,
                Address = e.Address,
                CreatedAt = e.CreatedAt,
                Countries = e.Countries.Select(e => new CountryRequest()
                {
                    CountryCode = e.CountryCode,
                    CountryName = e.CountryName,
                    PhoneCode = e.PhoneCode
                }).ToList(),
                Email = e.Email,
                Icon = e.Icon,
                Name = e.Name,
                Phone = e.Phone,
                RewardCategories = e.RewardCategories.Select(e => new RewardCategoryRequest()
                {
                    Description = e.Description,
                    Name = e.Name
                }).ToList(),

            }).FirstOrDefaultAsync(e => e.Id == id);

        public async Task<List<DataProviderRequest>> GETLIST() => await _context.DataProviders
            .Select(e => new DataProviderRequest()
            {
                Id = e.Id,
                Address = e.Address,
                CreatedAt = e.CreatedAt,
                Countries = e.Countries.Select(e => new CountryRequest()
                {
                    CountryCode = e.CountryCode,
                    CountryName = e.CountryName,
                    PhoneCode = e.PhoneCode
                }).ToList(),
                Email = e.Email,
                Icon = e.Icon,
                Name = e.Name,
                Phone = e.Phone,
                RewardCategories = e.RewardCategories.Select(e => new RewardCategoryRequest()
                {
                    Description = e.Description,
                    Name = e.Name
                }).ToList(),

            })
            .ToListAsync();

        public async Task<DataProviderRequest> PUT(Guid id, DataProviderRequest model)
        {
            var dataProvider = await _context.DataProviders.FirstOrDefaultAsync(e => e.Id == id);
            if (dataProvider == null)
            {
                return null;
            }
            else
            {
                dataProvider.Address = model.Address;
                dataProvider.CreatedAt = model.CreatedAt;
                dataProvider.Countries = model.Countries.Select(e => new Country()
                {
                    CountryCode = e.CountryCode,
                    CountryName = e.CountryName,
                    PhoneCode = e.PhoneCode
                }).ToList();
                dataProvider.Email = model.Email;
                dataProvider.Icon = model.Icon;
                dataProvider.Name = model.Name;
                dataProvider.Phone = model.Phone;
                _context.DataProviders.Update(dataProvider);
                await _context.SaveChangesAsync();
                return model;
            }
        }

        public async Task<DataProviderRequest> DELETE(Guid id)
        {
            var dataProvider = await _context.DataProviders.FirstOrDefaultAsync(e => e.Id == id); if (dataProvider == null)
            {
                return null;
            }
            else
            {
                _context.DataProviders.Remove(dataProvider);
                await _context.SaveChangesAsync();
                return new DataProviderRequest();
            }
        }

        public async Task<RewardCategoryRequest> RewardCategoryPOST(RewardCategoryRequest model)
        {
            var reward = await _context.RewardCategories.FirstOrDefaultAsync(e => e.Name == model.Name);
            {
                if (reward == null)
                {
                    reward = new RewardCategory()
                    {
                        Description = model.Description,
                        Name = model.Name
                    };
                    await _context.RewardCategories.AddAsync(reward);
                    await _context.SaveChangesAsync();
                    model.Id = reward.Id;
                    return model;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<RewardCategoryRequest> RewardCategoryGETBYID(Guid id) => await _context.RewardCategories
            .Select(e => new RewardCategoryRequest()
            {
                Description = e.Description,
                Id = e.Id,
                Name = e.Name,
            }).FirstOrDefaultAsync(e => e.Id == id);

        public async Task<List<RewardCategoryRequest>> RewardCategoryGETLIST() => await _context.RewardCategories
            .Select(e => new RewardCategoryRequest()
            {
                Description = e.Description,
                Id = e.Id,
                Name = e.Name,
            })
            .ToListAsync();

        public async Task<RewardCategoryRequest> RewardCategoryPUT(Guid id, RewardCategoryRequest model)
        {
            var rewardCategories = await _context.RewardCategories.FirstOrDefaultAsync(e => e.Id == id);
            if (rewardCategories == null)
            {
                return null;
            }
            else
            {
                rewardCategories.Name = model.Name;
                rewardCategories.Description = model.Description;
                _context.RewardCategories.Update(rewardCategories);
                await _context.SaveChangesAsync();
                return model;
            }
        }

        public async Task<RewardCategoryRequest> RewardCategoryDELETE(Guid id)
        {
            var reward = await _context.RewardCategories.FirstOrDefaultAsync(e => e.Id == id);
            if (reward == null)
            {
                return null;
            }
            else
            {
                _context.RewardCategories.Remove(reward);
                await _context.SaveChangesAsync();
                return new RewardCategoryRequest();
            }
        }

        public async Task<TransactionRequest> TransactionPOST(TransactionRequest model)
        {
            var tr = await _context.Transactions.FirstOrDefaultAsync(e => e.TxId == model.TxId);
            {
                if (tr == null)
                {
                    tr = new Transaction()
                    {
                        Amount = model.Amount,
                        From = model.From,
                        To = model.To,
                        TxDate = model.TxDate,
                        Type = model.Type,
                    };
                    await _context.Transactions.AddAsync(tr);
                    await _context.SaveChangesAsync();
                    model.TxId = tr.TxId;
                    return model;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<TransactionRequest> TransactionGETBYID(Guid id) => await _context.Transactions
           .Select(e => new TransactionRequest()
           {
               Amount = e.Amount,
               From = e.From,
               To = e.To,
               TxDate = e.TxDate,
               Type = e.Type,
               TxId = e.TxId
           }).FirstOrDefaultAsync(e => e.TxId == id);

        public async Task<List<TransactionRequest>> TransactionGETLIST() => await _context.Transactions
            .Select(e => new TransactionRequest()
            {
                Amount = e.Amount,
                From = e.From,
                To = e.To,
                TxDate = e.TxDate,
                Type = e.Type,
                TxId = e.TxId
            })
            .ToListAsync();

        public async Task<TransactionRequest> TransactionPUT(Guid id, TransactionRequest model)
        {
            var tr = await _context.Transactions.FirstOrDefaultAsync(e => e.TxId == id);
            if (tr == null)
            {
                return null;
            }
            else
            {
                tr.Amount = model.Amount;
                tr.From = model.From;
                tr.To = model.To;
                tr.TxDate = model.TxDate;
                tr.Type = model.Type;
                _context.Transactions.Update(tr);
                await _context.SaveChangesAsync();
                return model;
            }
        }

        public async Task<TransactionRequest> TransactionDELETE(Guid id)
        {
            var tr = await _context.Transactions.FirstOrDefaultAsync(e => e.TxId == id);
            if (tr == null)
            {
                return null;
            }
            else
            {
                _context.Transactions.Remove(tr);
                await _context.SaveChangesAsync();
                return new TransactionRequest();
            }
        }
    }
}

