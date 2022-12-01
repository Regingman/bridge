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

        public async Task<List<ProvaidersRequest>> GetUsersProvidersAsync(string countryCode, string userId)
        {
            var prodavaders = await _context.UserTermsOfUses.Where(e => e.UserId == Guid.Parse(userId)).ToListAsync();
            var country = await _context.Countries
                .Include(provider => provider.DataProviders)
                .Where(x => x.CountryCode == countryCode)
                .Select(x => new ProvaidersRequest(
                    )
                {
                    Connected = false,
                    CountryCode = x.CountryCode,
                    CountryName = x.CountryName,
                    DataProviders = x.DataProviders,
                    PhoneCode = x.PhoneCode
                })
                .ToListAsync();
            country.ForEach(e =>
            {
                if (e.DataProviders.Count() > 0)
                {
                    bool flag = false;
                    prodavaders.ForEach(x =>
                    {
                        flag = e.DataProviders.Where(e => e.Id == x.DataProviderId).Count() > 0;
                    });
                    e.Connected = flag;
                }
            });
            return country;
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

        public async Task<TermOfUse> TermOfUseStatus(string userFIO, Guid userId, Guid provaiderId)
        {
            var provaider = await _context.DataProviders.FirstOrDefaultAsync(e => e.Id == provaiderId);
            if (provaider == null)
            {
                return null;
            }
            var termResponse = new TermOfUse()
            {
                Flag = false,
                Text = GetTerms(userFIO, provaider.Name)
            };
            var useTerm = await _context.UserTermsOfUses.FirstOrDefaultAsync(e => e.UserId == userId && e.DataProviderId == provaiderId);
            if (useTerm == null)
            {
                useTerm = new UserTermsOfUse()
                {
                    UserId = userId,
                    DataProviderId = provaiderId,
                    IsRegistered = false
                };
                await _context.UserTermsOfUses.AddAsync(useTerm);
                await _context.SaveChangesAsync();
            }
            else if (useTerm.IsRegistered == false)
            {
                termResponse.Flag = false;
            }
            else
            {
                termResponse.Flag = true;
            }
            return termResponse;
        }

        public async Task<bool> TermOfUseApply(Guid userId, Guid provaiderId)
        {
            try
            {
                var useTerm = await _context.UserTermsOfUses.FirstOrDefaultAsync(e => e.UserId == userId && e.DataProviderId == provaiderId);
                if (useTerm == null)
                {
                    useTerm = new UserTermsOfUse()
                    {
                        UserId = userId,
                        DataProviderId = provaiderId,
                        IsRegistered = true
                    };
                    await _context.UserTermsOfUses.AddAsync(useTerm);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    useTerm.IsRegistered = true;
                    _context.UserTermsOfUses.Update(useTerm);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> TermOfUseCancel(Guid userId, Guid provaiderId)
        {
            try
            {
                var useTerm = await _context.UserTermsOfUses.FirstOrDefaultAsync(e => e.UserId == userId && e.DataProviderId == provaiderId);
                if (useTerm == null)
                {
                    useTerm = new UserTermsOfUse()
                    {
                        UserId = userId,
                        DataProviderId = provaiderId,
                        IsRegistered = false
                    };
                    await _context.UserTermsOfUses.AddAsync(useTerm);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    useTerm.IsRegistered = false;
                    _context.UserTermsOfUses.Update(useTerm);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<TransactionRequest>> GetStatistics(Guid userId)
         => await _context.Transactions.Where(e => Guid.Parse(e.To) == userId).Select(e => new TransactionRequest()
         {
             Amount = e.Amount,
             From = e.From,
             To = e.To,
             TxDate = e.TxDate,
             TxId = e.TxId,
             Type = e.Type,
         }).ToListAsync();


        public string GetTerms(string fio, string provaiderName)
        {
            return @"<html><body>

                    <h4>Terms</h4>
                    <p>By accessing the website at mydatacoin.io, you agree to be bound by these terms of service, all applicable laws, and regulations and agree that you are responsible for compliance with any applicable local laws. If you do not agree with these terms, you are prohibited from using or accessing this site. In addition, the materials contained in this website are protected by applicable copyright and trademark law.</p>


                    <br/><p></p>
                    <h4>Use License</h4>
                    <p>Permission is granted to temporarily download one copy of the materials (information or software) on myData.coin's website for only personal, non-commercial transitory viewing. This is the grant of a license, not a transfer of title, and under this license, you may not:</p>
                    <p>• Modify or copy the materials</p>
                    <p>• Use the materials for any commercial purpose, or any public display (commercial or non-commercial)</p>
                    <p>• Attempt to decompile or reverse engineer any software contained on MyDataCoin's website;</p>
                    <p>• Transfer the materials to another person or “mirror” the materials on any other server</p>
                    <p>This license shall automatically terminate if you violate any of these restrictions and may be terminated by myData.coin at any time. Upon terminating your viewing of these materials or upon the termination of this license, you must destroy any downloaded materials in your possession whether in electronic or printed format.</p>

                    <br/><p></p>
                    <h4>Disclaimer</h4>
                    <p>The materials on MyDataCoin's website are provided on an 'as is' basis. myData.coin makes no warranties, expressed or implied, and hereby disclaims and negates all other warranties including, without limitation, implied warranties or conditions of merchantability, fitness for a particular purpose, or non-infringement of intellectual property or other violation of rights.</p>
                    <p>Further, MyDataCoin does not warrant or make any representations concerning the accuracy, likely results, or reliability of the use of the materials on its website or otherwise relating to such materials or on any sites linked to this site.</p>
                    
                    <br/><p></p>
                    <h4>Limitations</h4>
                    <p>In no event shall myData.coin or its suppliers be liable for any damages (including, without limitation, damages for loss of data or profit, or due to business interruption) arising out of the use or inability to use the materials on MyDataCoin's website, even if myData.coin or a myData.coin authorised representative has been notified orally or in writing of the possibility of such damage. Because some jurisdictions do not allow limitations on implied warranties, or limitations of liability for consequential or incidental damages, these limitations may not apply to you.</p>

                    <br/><p></p>
                    <h4>Links</h4>
                    <p>MyDataCoin has not reviewed all of the sites linked to its website and is not responsible for the contents of any such linked site. The inclusion of any link does not imply endorsement by MyDataCoin of the site. Use of any such linked website is at the user's own risk. </p>

                    <br/><p></p>
                    <h4>Modifications</h4>
                    <p>MyDataCoin may revise these terms of service for its website at any time without notice. By using this website, you agree to be bound by the then current version of these terms of service.</p>
                    
                    <br/><p></p>
                    <h4>Governing Law</h4>
                    <p>MyDataCoin may revise these terms of service for its website at any time without notice. By using this website, you agree to be bound by the then current version of these terms of service.</p>

                    <br/><p></p><p>Latest update: 1 April 2022</p>

                    </body></html>";
        }

    }
}

