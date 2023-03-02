using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyDataCoin.Models;
using MyDataCoinBridge.DataAccess;
using MyDataCoinBridge.Entities;
using MyDataCoinBridge.Helpers;
using MyDataCoinBridge.Interfaces;
using MyDataCoinBridge.Models;
using MyDataCoinBridge.Models.Provider;
using MyDataCoinBridge.Models.TermsOfUse;
using MyDataCoinBridge.Models.Transaction;
using MyDataCoinBridge.Models.WebHooks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyDataCoinBridge.Services
{
    public class ProvidersServices : IProviders
    {
        private readonly WebApiDbContext _context;
        private readonly IHttpClientFactory _clientFactory;
        private readonly AppSettings _appSettings;
        private readonly ILogger<ProvidersServices> _logger;
        private static string Bucket = "mydatacoin.appspot.com";
        private static string AuthEmail = "img@gmail.com";

        public ProvidersServices(
            IHttpClientFactory clientFactory,
            IOptions<AppSettings> appSettings,
            WebApiDbContext context,
            ILogger<ProvidersServices> logger)
        {
            _appSettings = appSettings.Value;
            _clientFactory = clientFactory;
            _context = context;
            _logger = logger;
        }

        public async Task<DataProvider> GetProviderByIdAsync(Guid id)
        {
            return await _context.DataProviders.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ProvidersListResponse> GetUsersProvidersAsync(string countryCode, string userId)
        {
            try
            {
                var providers = await _context.UserTermsOfUses.Where(e => e.UserId == Guid.Parse(userId)).ToListAsync();
                var country = await _context.Countries
                    .Include(provider => provider.DataProviders)
                    .Where(x => x.CountryCode == countryCode)
                    .Select(x => new ProvidersRequest(
                        )
                    {
                        CountryCode = x.CountryCode,
                        CountryName = x.CountryName,
                        DataProviders = x.DataProviders.Select(e => new DataProviderRequest()
                        {
                            Id = e.Id, Address = e.Address, CreatedAt = e.CreatedAt,
                            Countries = e.Countries.Select(e => new CountryRequest()
                            {
                                CountryCode = e.CountryCode,
                                CountryName = e.CountryName,
                                PhoneCode = e.PhoneCode
                            }).ToList(),
                            Email = e.Email, Icon = e.Icon, Name = e.Name, Phone = e.Phone,
                            RewardCategories = e.RewardCategories.Select(e => new RewardCategoryRequest()
                            {
                                Description = e.Description,
                                Name = e.Name
                            }).ToList(),

                        }).ToList(),
                        PhoneCode = x.PhoneCode
                    })
                    .ToListAsync();
                country.ForEach(e =>
                {
                    if (e.DataProviders.Count() > 0)
                    {
                        bool flag = false;
                        foreach (var temp in e.DataProviders)
                        {
                            flag = providers.Where(x => x.DataProviderId == temp.Id && x.IsRegistered).Count() > 0;
                            temp.Connected = flag;
                        }
                    }
                });

                var list = await _context.BridgeTransactions.Where(e => e.UserId == userId && !e.Claim).ToListAsync();

                if(providers == null)
                    return new ProvidersListResponse(204, "Providers Not Found");

                if(country == null)
                    return new ProvidersListResponse(204, "Countries Not Found");

                if(list == null)
                    return new ProvidersListResponse(200, "", country, false);
                else
                    return new ProvidersListResponse(200, "", country, true);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ProvidersListResponse(400, ex.Message);
            }
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


        public async Task<DataProviderRequest> POST(DataProviderRequest model)
        {
            var dataProvider = await _context.DataProviders.FirstOrDefaultAsync(e => e.Email == model.Email);
            {
                if (dataProvider == null)
                {
                    var user = await _context.BridgeUsers.FirstOrDefaultAsync(e => e.TokenForService == model.Token);
                    if (user == null) return null;
                    var countries = await _context.Countries.ToListAsync();
                    var rewards = await _context.RewardCategories.ToListAsync();
                    dataProvider = new DataProvider()
                    {
                        Address = model.Address,
                        CreatedAt = DateTime.UtcNow,
                        Countries = model.Countries.Select(e => countries.FirstOrDefault(x => x.Id == e.Id)).ToList(),
                        Email = model.Email,
                        Icon = model.Icon,
                        Name = model.Name,
                        Phone = model.Phone,
                        BridgeUserId = user.Id,
                        RewardCategories = model.RewardCategories.Select(e => rewards.FirstOrDefault(x => x.Id == e.Id)).ToList(),
                    };
                    await _context.DataProviders.AddAsync(dataProvider);
                    await _context.SaveChangesAsync();
                    model.Id = dataProvider.Id;
                    return new DataProviderRequest()
                    {
                        Id = dataProvider.Id,
                        Address = dataProvider.Address,
                        CreatedAt = dataProvider.CreatedAt,
                        Countries = dataProvider.Countries.Select(e => new CountryRequest()
                        {
                            Id = e.Id,
                            CountryCode = e.CountryCode,
                            CountryName = e.CountryName,
                            PhoneCode = e.PhoneCode
                        }).ToList(),
                        Email = dataProvider.Email,
                        Icon = dataProvider.Icon,
                        Name = dataProvider.Name,
                        Phone = dataProvider.Phone,
                        Token = dataProvider.BridgeUser.TokenForService,
                        RewardCategories = dataProvider.RewardCategories.Select(e => new RewardCategoryRequest()
                        {
                            Id = e.Id,
                            Description = e.Description,
                            Name = e.Name
                        }).ToList()
                    }; ;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<DataProviderRequest> GETBYID(string token) => await _context.DataProviders
            .Include(e => e.BridgeUser)
            .Select(e => new DataProviderRequest()
            {
                Id = e.Id,
                Address = e.Address,
                CreatedAt = e.CreatedAt,
                Countries = e.Countries.Select(e => new CountryRequest()
                {
                    Id = e.Id,
                    CountryCode = e.CountryCode,
                    CountryName = e.CountryName,
                    PhoneCode = e.PhoneCode
                }).ToList(),
                Email = e.Email,
                Icon = e.Icon,
                Name = e.Name,
                Phone = e.Phone,
                Token = e.BridgeUser.TokenForService,
                IsVerified = e.BridgeUser.IsVerified,
                RewardCategories = e.RewardCategories.Select(e => new RewardCategoryRequest()
                {
                    Id = e.Id,
                    Description = e.Description,
                    Name = e.Name
                }).ToList(),

            }).FirstOrDefaultAsync(e => e.Token == token);

        public async Task<List<CountryRequest>> GETLISTCountry() => await _context.Countries
            .Select(e => new CountryRequest()
            {
                Id = e.Id,
                CountryCode = e.CountryCode,
                CountryName = e.CountryName,
                PhoneCode = e.PhoneCode
            })
            .ToListAsync();

        public async Task<List<DataProviderRequest>> GETLIST() => await _context
            .DataProviders
            .Where(e => e.BridgeUserId != null)
            .Include(e => e.BridgeUser)
            .Include(e => e.RewardCategories)
            .Include(e => e.Countries)
            .Select(e => new DataProviderRequest()
            {
                Id = e.Id,
                Address = e.Address,
                CreatedAt = e.CreatedAt,
                Countries = e.Countries.Select(e => new CountryRequest()
                {
                    Id = e.Id,
                    CountryCode = e.CountryCode,
                    CountryName = e.CountryName,
                    PhoneCode = e.PhoneCode
                }).ToList(),
                Email = e.Email,
                Icon = e.Icon,
                Name = e.Name,
                Phone = e.Phone,
                IsVerified = e.BridgeUser.IsVerified,
                BridgeUserEmail = e.BridgeUser.Email,
                RewardCategories = e.RewardCategories.Select(e => new RewardCategoryRequest()
                {
                    Id = e.Id,
                    Description = e.Description,
                    Name = e.Name
                }).ToList(),
            })
            .ToListAsync();

        public async Task<DataProviderRequest> PUT(Guid id, DataProviderRequest model)
        {
            var countries = await _context.Countries
                .Include(e => e.DataProviders).ToListAsync();
            var rewards = await _context.RewardCategories
                .Include(e => e.DataProviders)
                .ToListAsync();
            var dataProvider = await _context.DataProviders.FirstOrDefaultAsync(e => e.Id == id);
            if (dataProvider == null)
            {
                return null;
            }
            else
            {
                var user = await _context.BridgeUsers.FirstOrDefaultAsync(e => e.TokenForService == model.Token);
                if (user == null) return null;

                dataProvider.Address = model.Address;
                dataProvider.Countries = null;
                dataProvider.Email = model.Email;
                dataProvider.BridgeUserId = user.Id;
                dataProvider.Icon = model.Icon;
                dataProvider.Name = model.Name;
                dataProvider.RewardCategories = null;
                dataProvider.Phone = model.Phone;
                rewards.ForEach(e =>
                {
                    foreach (var temp in e.DataProviders)
                    {
                        if (temp.Id == dataProvider.Id)
                        {
                            e.DataProviders.Remove(temp);
                        }
                    }
                });
                countries.ForEach(e =>
                {
                    foreach (var temp in e.DataProviders)
                    {
                        if (temp.Id == dataProvider.Id)
                        {
                            e.DataProviders.Remove(temp);
                        }
                    }
                });
                _context.DataProviders.Update(dataProvider);
                _context.Countries.UpdateRange(countries);
                _context.RewardCategories.UpdateRange(rewards);
                await _context.SaveChangesAsync();
                dataProvider = await _context.DataProviders.FirstOrDefaultAsync(e => e.Id == id);
                dataProvider.Countries = model.Countries.Select(e => countries.FirstOrDefault(x => x.Id == e.Id)).ToList();
                dataProvider.RewardCategories = model.RewardCategories.Select(e => rewards.FirstOrDefault(x => x.Id == e.Id)).ToList();

                _context.DataProviders.Update(dataProvider);
                await _context.SaveChangesAsync();

                return new DataProviderRequest()
                {
                    Id = dataProvider.Id,
                    Address = dataProvider.Address,
                    CreatedAt = dataProvider.CreatedAt,
                    Countries = dataProvider.Countries.Select(e => new CountryRequest()
                    {
                        Id = e.Id,
                        CountryCode = e.CountryCode,
                        CountryName = e.CountryName,
                        PhoneCode = e.PhoneCode
                    }).ToList(),
                    Email = dataProvider.Email,
                    Icon = dataProvider.Icon,
                    Name = dataProvider.Name,
                    Phone = dataProvider.Phone,
                    Token = dataProvider.BridgeUser.TokenForService,
                    RewardCategories = dataProvider.RewardCategories.Select(e => new RewardCategoryRequest()
                    {
                        Id = e.Id,
                        Description = e.Description,
                        Name = e.Name
                    }).ToList()
                };
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

        // public async Task<TransactionRequest> TransactionPOST(TransactionRequest model)
        // {
        //     var tr = await _context.Transactions.FirstOrDefaultAsync(e => e.TxId == model.TxId);
        //     {
        //         if (tr == null)
        //         {
        //             tr = new Transaction()
        //             {
        //                 Amount = model.Amount,
        //                 From = model.From,
        //                 To = model.To,
        //                 TxDate = model.TxDate,
        //                 Type = TransactionHelpers.TransactionTypeToInt(model.Type),
        //             };
        //             await _context.Transactions.AddAsync(tr);
        //             await _context.SaveChangesAsync();
        //             model.TxId = tr.TxId;
        //             return model;
        //         }
        //         else
        //         {
        //             return null;
        //         }
        //     }
        // }

        // public async Task<TransactionRequest> TransactionGETBYID(Guid id) => await _context.Transactions
        //    .Select(e => new TransactionRequest()
        //    {
        //        Amount = e.Amount,
        //        From = e.From,
        //        To = e.To,
        //        TxDate = e.TxDate,
        //        Type = TransactionHelpers.TransactionType(e.Type),
        //        TxId = e.TxId
        //    }).FirstOrDefaultAsync(e => e.TxId == id);

        // public async Task<List<TransactionRequest>> TransactionGETLIST() => await _context.Transactions
        //     .Select(e => new TransactionRequest()
        //     {
        //         Amount = e.Amount,
        //         From = e.From,
        //         To = e.To,
        //         TxDate = e.TxDate,
        //         Type = TransactionHelpers.TransactionType(e.Type),
        //         TxId = e.TxId
        //     })
        //     .ToListAsync();

        // public async Task<TransactionRequest> TransactionPUT(Guid id, TransactionRequest model)
        // {
        //     var tr = await _context.Transactions.FirstOrDefaultAsync(e => e.TxId == id);
        //     if (tr == null)
        //     {
        //         return null;
        //     }
        //     else
        //     {
        //         tr.Amount = model.Amount;
        //         tr.From = model.From;
        //         tr.To = model.To;
        //         tr.TxDate = model.TxDate;
        //         tr.Type = TransactionHelpers.TransactionTypeToInt(model.Type);
        //         _context.Transactions.Update(tr);
        //         await _context.SaveChangesAsync();
        //         return model;
        //     }
        // }

        // public async Task<TransactionRequest> TransactionDELETE(Guid id)
        // {
        //     var tr = await _context.Transactions.FirstOrDefaultAsync(e => e.TxId == id);
        //     if (tr == null)
        //     {
        //         return null;
        //     }
        //     else
        //     {
        //         _context.Transactions.Remove(tr);
        //         await _context.SaveChangesAsync();
        //         return new TransactionRequest();
        //     }
        // }

        public async Task<TermOfUse> TermOfUseStatus(Guid userId, Guid provaiderId, List<string> email, List<string> phone)
        {
            var provaider = await _context.DataProviders.FirstOrDefaultAsync(e => e.Id == provaiderId);
            if (provaider == null)
            {
                return null;
            }
            var termResponse = new TermOfUse()
            {
                Flag = false,
                Logo = provaider.Icon,
                ProviderName = provaider.Name,
                Text = GetTerms("", provaider.Name),
            };
            var useTerm = await _context.UserTermsOfUses.FirstOrDefaultAsync(e => e.UserId == userId && e.DataProviderId == provaiderId);
            if (useTerm == null)
            {
                useTerm = new UserTermsOfUse()
                {
                    UserId = userId,
                    DataProviderId = provaiderId,
                    IsRegistered = false,
                    Email = email,
                    Phone = phone
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

        // public async Task<TermOfUse> TermOfUseStatus(string userFIO, Guid userId, Guid provaiderId)
        // {
        //     var provaider = await _context.DataProviders.FirstOrDefaultAsync(e => e.Id == provaiderId);
        //     if (provaider == null)
        //     {
        //         return null;
        //     }
        //     var termResponse = new TermOfUse()
        //     {
        //         Flag = false,
        //         Logo = provaider.Icon,
        //         ProviderName = provaider.Name,
        //         Text = GetTerms(userFIO, provaider.Name),
        //     };
        //     var useTerm = await _context.UserTermsOfUses.FirstOrDefaultAsync(e => e.UserId == userId && e.DataProviderId == provaiderId);
        //     if (useTerm == null)
        //     {
        //         useTerm = new UserTermsOfUse()
        //         {
        //             UserId = userId,
        //             DataProviderId = provaiderId,
        //             IsRegistered = false,
        //         };
        //         await _context.UserTermsOfUses.AddAsync(useTerm);
        //         await _context.SaveChangesAsync();
        //     }
        //     else if (useTerm.IsRegistered == false)
        //     {
        //         termResponse.Flag = false;
        //     }
        //     else
        //     {
        //         termResponse.Flag = true;
        //     }
        //     return termResponse;
        // }

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
                _logger.LogError(ex.Message);
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
                _logger.LogError(ex.Message);
                return false;
            }
        }



        public string GetTerms(string fio, string providerName)
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

        public async Task<GeneralResponse> TransactionAddProvider(string token, List<TransactionProviderRequest> model)
        {
            var rewards = await _context.RewardCategoryByProviders
                .Include(e => e.RewardCategory)
                .Include(e => e.DataProvider)
                .Include(e => e.DataProvider.BridgeUser)
                .Where(e => e.DataProvider.BridgeUser.TokenForService == token).ToListAsync();
            var list = new List<BridgeTransaction>();
            var users = await _context.UserTermsOfUses.Where(e => e.Email != null && e.Phone != null).ToListAsync();
            var provider = await _context.DataProviders
                .Include(e => e.BridgeUser)
                .FirstOrDefaultAsync(e => e.BridgeUser.TokenForService == token);
            try
            {
                if (provider == null)
                {
                    return new GeneralResponse(400, "Sorry, you send inccorect token! Pls try again!");
                }
                else
                {
                    foreach (var temp in model)
                    {
                        //TODO to lower add all users
                        var user = users.FirstOrDefault(e => (e.Email.Contains(temp.EmailPhone.ToLower()) ||
                        e.Phone.Contains(temp.EmailPhone.ToLower())
                        && e.IsRegistered
                        && e.DataProviderId == provider.Id));
                        if (user != null)
                        {
                            var reward = rewards.FirstOrDefault(r => r.RewardCategoryId == Guid.Parse(temp.RewardCategoryId));
                            if (reward != null)
                            {
                                var transaction =
                                    new BridgeTransaction()
                                    {
                                        Count = temp.Count,
                                        Created = DateTime.UtcNow,
                                        Email = user.Email.Contains(temp.EmailPhone) ? temp.EmailPhone : null,
                                        Phone = user.Phone.Contains(temp.EmailPhone) ? temp.EmailPhone : null,
                                        ProviderId = provider.Id.ToString(),
                                        ProviderName = provider.Name,
                                        RewardCategoryId = reward.Id.ToString(),
                                        RewardCategoryName = reward.RewardCategory.Name,
                                        USDMDC = temp.Count * reward.Amount,
                                        UserId = user.UserId.ToString()
                                    };
                                transaction.Commission = transaction.USDMDC / 10;
                                transaction.Claim = false;
                                list.Add(transaction);
                            }
                        }
                    }
                    await _context.BridgeTransactions.AddRangeAsync(list);
                    await _context.SaveChangesAsync();
                    return new GeneralResponse(200, list.Select(e => new
                    {
                        Email = e.Email,
                        Phone = e.Phone,
                        RewardCategory = e.RewardCategoryName,
                        Count = e.Count,
                        USDMDC = e.USDMDC
                    }));
                }
            }
            catch (Exception ex)
            {
                return new GeneralResponse(400, ex.Message);
            }
        }

        public async Task<List<TransactionProviderResponse>> GetStatisticFromProvider(string token)
        {
            var provider = await _context.DataProviders
               .Include(e => e.BridgeUser)
               .FirstOrDefaultAsync(e => e.BridgeUser.TokenForService == token);
            if (provider == null)
            {
                return null;
            }
            else
            {
                return await _context.BridgeTransactions.Where(e => e.ProviderId == provider.Id.ToString()).Select(e => new TransactionProviderResponse()
                {
                    USDMCDAmount = e.USDMDC,
                    Count = e.Count,
                    Created = e.Created,
                    EmailPhone = e.Phone == null ? e.Email : e.Phone,
                    RewardCategoryName = e.RewardCategoryName,
                }).ToListAsync();
            }
        }

        public async Task<List<TransactionProviderResponse>> GetStatisticFromProviderFromAdmin()
        {
            return await _context.BridgeTransactions.Select(e => new TransactionProviderResponse()
            {
                USDMCDAmount = e.USDMDC,
                Count = e.Count,
                Created = e.Created,
                EmailPhone = e.Phone == null ? e.Email : e.Phone,
                RewardCategoryName = e.RewardCategoryName,
            }).ToListAsync();
        }

        public async Task<decimal> Claim(string userId)
        {
            var list = await _context.BridgeTransactions.Where(e => e.UserId == userId && !e.Claim).ToListAsync();
            foreach (var temp in list)
            {
                temp.Claim = true;
            }
            _context.BridgeTransactions.UpdateRange(list);
            await _context.SaveChangesAsync();
            return list.Select(e => e.USDMDC).Sum();
        }

        public async Task<AllDataFromStatisticRequest> GetStatisticsExtend(string userId)
        {
            var transactions = await _context.BridgeTransactions
                  .Where(e => e.UserId == userId && e.Claim)
                  .Select(e => new TransactionRequest()
                  {
                      Amount = e.USDMDC,
                      From = e.ProviderName,
                      To = e.UserId,
                      TxDate = e.Created,
                      TxId = e.Id,
                      Type = e.RewardCategoryName,
                  })
                  .ToListAsync();

            var earnsList
                = transactions
                .GroupBy(e => e.Type)
                .Select(e => new TotalEarned()
                {
                    Amount = 0,
                    Name = e.Key
                })
                .ToList();

            foreach (var temp in earnsList)
            {
                temp.Amount += transactions.Where(e => e.Type == temp.Name).Sum(e => e.Amount);
            }

            return new AllDataFromStatisticRequest
            {
                TotalEarneds = earnsList,
                TotalTransactions = transactions
            };
        }

        public async Task<DataProvider> GetProviderByToken(string token)
        {
            var user = await _context.BridgeUsers.FirstOrDefaultAsync(e => e.TokenForService == token);
            if (user == null)
            {
                return null;
            }
            return await _context.DataProviders.Include(e => e.BridgeUser).FirstOrDefaultAsync(e => e.BridgeUserId == user.Id);
        }

        public async Task<GeneralResponse> Upload(Uploadrequest model)
        {
            byte[] bytes = Convert.FromBase64String(model.ImageData);
            MemoryStream stream = new MemoryStream(bytes);
            var auth = new FirebaseAuthProvider(new FirebaseConfig(_appSettings.G_API_KEY));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, _appSettings.G_API_PASSWORD);
            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(
                Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true // when you cancel the upload, exception is thrown. By default no exception is thrown
                })
                .Child("Images")
                .Child($"{model.Token}.{model.Extension}")
                .PutAsync(stream, cancellation.Token);

            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            try
            {
                var user = await _context.DataProviders
                    .Include(e => e.BridgeUser)
                    .SingleOrDefaultAsync(x => x.BridgeUser.TokenForService == model.Token);
                string add = $"?alt=media&token={_appSettings.G_IMAGE_TOKEN}";

                string[] words = task.TargetUrl.Split('?');
                string[] words2 = words[1].Split('=');
                string finalString = words[0] + "/" + words2[1] + add;

                user.Icon = finalString;
                _context.DataProviders.Update(user);
                await _context.SaveChangesAsync();
                return new GeneralResponse(200, finalString);
            }
            catch (Exception ex)
            {
                return new GeneralResponse(400, ex.Message);
            }
        }

        public async Task<DataProvider> LogoUpload(string path, DataProvider provider)
        {
            provider.Icon = path;
            _context.DataProviders.Update(provider);
            await _context.SaveChangesAsync();
            return provider;
        }

        public async Task<GeneralResponse> GetUserInfo(UserInfoModel model)
        {
            if (model.Action == 0)
            {
                try
                {
                    if (model.Server_Secret != _appSettings.SERVER_KEY)
                    {
                        return new GeneralResponse(400, "Error!");
                    }

                    var provider = await _context.DataProviders
                        .Include(e => e.BridgeUser)
                        .FirstOrDefaultAsync(e => e.Id == model.ProviderId);
                    if (provider == null)
                    {
                        return new GeneralResponse(400, "Provider Not Found!");
                    }

                    var hook = await _context.WebHooks.FirstOrDefaultAsync(e => e.Secret == provider.BridgeUser.Secret);
                    if (hook == null)
                    {
                        return new GeneralResponse(400, "Web Hook Not Found!");
                    }
                    if (!hook.IsActive)
                    {
                        return new GeneralResponse(400, "Web Hook Not Active!");
                    }

                    var client = _clientFactory.CreateClient();
                    ProviderInfoModel modelUp = new ProviderInfoModel();
                    modelUp.Action = model.Action;
                    modelUp.Email = model.Emails;
                    modelUp.Phone = model.Phones;
                    modelUp.Secret = provider.BridgeUser.Secret;

                    string json = JsonConvert.SerializeObject(modelUp);
                    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"{hook.WebHookUrl}", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            var result = await response.Content.ReadAsStringAsync();
                            UserPrivacyProfileModel responseModel = JsonConvert.DeserializeObject<UserPrivacyProfileModel>(result);

                            UserPrivacySetting settings = await GetUserPrivacySettingsAsync(model.Emails[0]);

                            responseModel.UserPrivacySettings = settings;

                            return new GeneralResponse(200, responseModel);
                        }
                        catch (Exception e)
                        {
                            return new GeneralResponse(400, e.Message);
                        }
                    }
                    else
                    {
                        return new GeneralResponse(400, "");
                    }
                }
                catch (Exception ex)
                {
                    return new GeneralResponse(400, ex.Message);
                }
            }
            else
            {
                try
                {
                    if (model.Server_Secret != _appSettings.SERVER_KEY)
                    {
                        return new GeneralResponse(400, "Error!");
                    }

                    var provider = await _context.DataProviders
                        .Include(e => e.BridgeUser)
                        .FirstOrDefaultAsync(e => e.Id == model.ProviderId);
                    if (provider == null)
                    {
                        return new GeneralResponse(400, "Provider Not Found!");
                    }

                    var hook = await _context.WebHooks.FirstOrDefaultAsync(e => e.Secret == provider.BridgeUser.Secret);
                    if (hook == null)
                    {
                        return new GeneralResponse(400, "Web Hook Not Found!");
                    }
                    if (!hook.IsActive)
                    {
                        return new GeneralResponse(400, "Web Hook Not Active!");
                    }

                    var client = _clientFactory.CreateClient();
                    ProviderInfoModelProvicy modelUp = new ProviderInfoModelProvicy();
                    modelUp.Action = model.Action;
                    modelUp.Secret = provider.BridgeUser.Secret;
                    modelUp.Video_VAST_desktop = model.Video_VAST_desktop;
                    modelUp.Video_VAST_mobile = model.Video_VAST_mobile;
                    modelUp.Multitag_mobile = model.Multitag_mobile;
                    modelUp.In_page_desktop = model.In_page_desktop;
                    modelUp.Credit_History = model.Credit_History;
                    modelUp.Banner_mobile = model.Banner_mobile;
                    modelUp.Personal_Data = model.Personal_Data;
                    modelUp.Banner_desktop = model.Banner_desktop;
                    modelUp.Conversion = model.Conversion;
                    modelUp.Popunder_desktop = model.Popunder_desktop;
                    modelUp.Multitag_desktop = model.Multitag_desktop;
                    modelUp.View = model.View;
                    modelUp.Insurance_History = model.Insurance_History;
                    modelUp.Popunder_mobile = model.Popunder_mobile;
                    modelUp.Click = model.Click;
                    modelUp.In_page_mobile = model.In_page_mobile;
                    string json = JsonConvert.SerializeObject(modelUp);
                    var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"{hook.WebHookUrl}", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            return new GeneralResponse(200, "Ok");
                        }
                        catch (Exception e)
                        {
                            return new GeneralResponse(400, e.Message);
                        }
                    }
                    else
                    {
                        return new GeneralResponse(400, "");
                    }
                }
                catch (Exception ex)
                {
                    return new GeneralResponse(400, ex.Message);
                }
            }
        }

        public async Task<GeneralResponse> GetUserInfoProvicy(UserInfoModel model)
        {
            try
            {
                if (model.Server_Secret != _appSettings.SERVER_KEY)
                {
                    return new GeneralResponse(400, "Error!");
                }

                var provider = await _context.DataProviders
                    .Include(e => e.BridgeUser)
                    .FirstOrDefaultAsync(e => e.Id == model.ProviderId);
                if (provider == null)
                {
                    return new GeneralResponse(400, "Provider Not Found!");
                }

                var hook = await _context.WebHooks.FirstOrDefaultAsync(e => e.Secret == provider.BridgeUser.Secret);
                if (hook == null)
                {
                    return new GeneralResponse(400, "Web Hook Not Found!");
                }
                if (!hook.IsActive)
                {
                    return new GeneralResponse(400, "Web Hook Not Active!");
                }

                var client = _clientFactory.CreateClient();
                ProviderInfoModel modelUp = new ProviderInfoModel();
                modelUp.Action = model.Action;
                modelUp.Email = model.Emails;
                modelUp.Phone = model.Phones;
                modelUp.Secret = provider.BridgeUser.Secret;

                string json = JsonConvert.SerializeObject(modelUp);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{hook.WebHookUrl}", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        ProviderInfoModelProvicy responseModel = JsonConvert.DeserializeObject<ProviderInfoModelProvicy>(result);
                        responseModel.Action = model.Action;
                        return new GeneralResponse(200, responseModel);
                    }
                    catch (Exception e)
                    {
                        return new GeneralResponse(400, e.Message);
                    }
                }
                else
                {
                    return new GeneralResponse(400, "");
                }
            }
            catch (Exception ex)
            {
                return new GeneralResponse(400, ex.Message);
            }
        }

        public async Task<RewardCategoryByProviderRequest> RewardCategoryByProviderPOST(RewardCategoryByProviderRequest model)
        {
            var provider = await _context.DataProviders
                .Include(e => e.BridgeUser)
                .FirstOrDefaultAsync(e => e.BridgeUser.TokenForService == model.Token);

            if (provider == null)
            {
                return null;
            }
            else
            {
                var reward = new RewardCategoryByProvider()
                {
                    Amount = model.Amount,
                    DataProviderId = provider.Id,
                    RewardCategoryId = model.RewardCategoryId,
                    Id = Guid.NewGuid(),
                };

                await _context.RewardCategoryByProviders.AddAsync(reward);
                await _context.SaveChangesAsync();
                model.Id = reward.Id;
                return model;
            }
        }

        public async Task<RewardCategoryByProviderRequest> RewardCategoryByProviderGETBYID(Guid id, string token)
        {
            var provider = await _context.DataProviders
                .Include(e => e.BridgeUser)
                .FirstOrDefaultAsync(e => e.BridgeUser.TokenForService == token);

            if (provider == null)
            {
                return null;
            }
            else
            {
                var reward = await _context.RewardCategoryByProviders
                    .Include(e => e.RewardCategory)
                    .Include(e => e.DataProvider.BridgeUser)
                    .Include(e => e.DataProvider.BridgeUser)
                    .FirstOrDefaultAsync(e => e.DataProvider.BridgeUser.TokenForService == token && e.Id == id);

                return new RewardCategoryByProviderRequest()
                {
                    Amount = reward.Amount,
                    Id = reward.Id,
                    RewardCategoryId = reward.RewardCategoryId,
                    RewardCategoryName = reward.RewardCategory.Name
                };
            }
        }

        public async Task<List<RewardCategoryByProviderRequest>> RewardCategoryByProviderGETLIST(string token)
        {
            var provider = await _context.DataProviders
                .Include(e => e.BridgeUser)
                .FirstOrDefaultAsync(e => e.BridgeUser.TokenForService == token);

            if (provider == null)
            {
                return null;
            }
            else
            {

                return await _context.RewardCategoryByProviders
                    .Include(e => e.RewardCategory)
                    .Include(e => e.DataProvider.BridgeUser)
                    .Include(e => e.DataProvider.BridgeUser)
                    .Where(e => e.DataProvider.BridgeUser.TokenForService == token).Select(e => new RewardCategoryByProviderRequest()
                    {
                        Amount = e.Amount,
                        Id = e.Id,
                        RewardCategoryId = e.RewardCategoryId,
                        RewardCategoryName = e.RewardCategory.Name
                    }).ToListAsync();
            }
        }

        public async Task<RewardCategoryByProviderRequest> RewardCategoryByProviderPUT(Guid id, RewardCategoryByProviderRequest model)
        {
            var provider = await _context.DataProviders
                .Include(e => e.BridgeUser)
                .FirstOrDefaultAsync(e => e.BridgeUser.TokenForService == model.Token);

            if (provider == null)
            {
                return null;
            }
            else
            {
                var reward = await _context.RewardCategoryByProviders.FirstOrDefaultAsync(e => e.Id == id);
                if (reward == null)
                {
                    return null;
                }
                reward.Amount = model.Amount;
                reward.DataProviderId = provider.Id;
                reward.RewardCategoryId = model.RewardCategoryId;

                _context.RewardCategoryByProviders.Update(reward);
                await _context.SaveChangesAsync();
                model.Id = reward.Id;
                return model;
            }
        }

        public async Task<RewardCategoryByProviderRequest> RewardCategoryByProviderDELETE(Guid id, string token)
        {
            var provider = await _context.DataProviders
                  .Include(e => e.BridgeUser)
                  .FirstOrDefaultAsync(e => e.BridgeUser.TokenForService == token);

            if (provider == null)
            {
                return null;
            }
            else
            {
                var reward = await _context.RewardCategoryByProviders.FirstOrDefaultAsync(e => e.Id == id);
                if (reward == null)
                {
                    return null;
                }

                _context.Remove(reward);
                await _context.SaveChangesAsync();

                return new RewardCategoryByProviderRequest()
                {
                    Amount = reward.Amount,
                    Id = reward.Id,
                    RewardCategoryId = reward.RewardCategoryId,
                    RewardCategoryName = reward.RewardCategory.Name
                };
            }

        }

        public async Task<GeneralResponse> ChangePrivacySettings(ChangePrivacySettingsRequest model)
        {
            var user = await _context.UserPrivacySettings.SingleOrDefaultAsync(x => x.Email == model.Email);

            if(user == null)
                return new GeneralResponse(204, "User Not Found");

            switch(model.Category)
            {
                case "Profile":
                user.Profile = model.SettingValue;
                break;
                case "BasicData":
                user.BasicData = model.SettingValue;
                break;
                case "Contacts":
                user.Contacts = model.SettingValue;
                break;
                case "WorkAndEducation":
                user.WorkAndEducation = model.SettingValue;
                break;
                case "PlaceOfResidence":
                user.PlaceOfResidence = model.SettingValue;
                break;
                case "PersonalInterests":
                user.PersonalInterests = model.SettingValue;
                break;
            }

            await _context.SaveChangesAsync();

            return new GeneralResponse(200, "Ok");
        }

        private async Task<UserPrivacySetting> GetUserPrivacySettingsAsync(string email)
        {
            var res = await _context.UserPrivacySettings.SingleOrDefaultAsync(x => x.Email == email);

            if(res == null)
            {
                UserPrivacySetting userPrivacy = new UserPrivacySetting();
                userPrivacy.Id = new Guid();
                userPrivacy.Email = email;

                await _context.AddAsync(userPrivacy);
                await _context.SaveChangesAsync();

                return userPrivacy;
            }

            else
                return res;
        }
    }
}

