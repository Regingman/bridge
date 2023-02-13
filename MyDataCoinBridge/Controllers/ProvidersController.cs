using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyDataCoinBridge.Entities;
using MyDataCoinBridge.Interfaces;
using MyDataCoinBridge.Models;
using MyDataCoinBridge.Models.TermsOfUse;
using MyDataCoinBridge.Models.Transaction;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using static MyDataCoinBridge.Models.GoogleMainModel;

namespace MyDataCoinBridge.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProvidersController : ControllerBase
    {
        private readonly ILogger<ProvidersController> _logger;
        private readonly IProviders _provider;
        private readonly ClientInfo _clientInfo;
        private readonly ClientInfoFacebook _clientInfoFB;
        private readonly IHttpClientFactory _clientFactory;

        public ProvidersController(ILogger<ProvidersController> logger, IProviders provider)
        {
            _logger = logger;
            _provider = provider;
        }


        /// <summary>
        /// Get provider by id
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(DataProvider))]
        [Authorize]
        [HttpGet]
        [Route("get-provider-by-id/{id}")]
        public async Task<IActionResult> GetDataProviderById(string id)
        {
            var result = await _provider.GetProviderByIdAsync(Guid.Parse(id));
            return Ok(result);
        }

        /// <summary>
        /// Get providers by country code (KG, KZ)
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<Country>))]
        [Authorize]
        [HttpGet]
        [Route("get-users-providers/{countryCode}/{userId}")]
        public async Task<IActionResult> GetUsersProviders(string countryCode, string userId)
        {
            var result = await _provider.GetUsersProvidersAsync(countryCode, userId);
            return Ok(result);
        }


        #region Obsolete methods
        /// <summary>
        /// LoginGoogle
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [Obsolete]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [Route("LoginGoogle")]
        [HttpGet]
        public IActionResult LoginGoogle()
        {
            var ClientId = _clientInfo.Client.client_id;
            var stringTestUrl = HttpUtility.UrlEncode("profile " +
                "https://www.googleapis.com/auth/contacts " +
                "https://www.googleapis.com/auth/contacts.readonly " +
                "https://www.googleapis.com/auth/directory.readonly " +
                "https://www.googleapis.com/auth/profile.agerange.read " +
                "https://www.googleapis.com/auth/profile.emails.read " +
                "https://www.googleapis.com/auth/profile.language.read " +
                "https://www.googleapis.com/auth/user.addresses.read " +
                "https://www.googleapis.com/auth/user.birthday.read " +
                "https://www.googleapis.com/auth/user.emails.read " +
                "https://www.googleapis.com/auth/user.gender.read " +
                "https://www.googleapis.com/auth/user.organization.read " +
                "https://www.googleapis.com/auth/user.phonenumbers.read " +
                "https://www.googleapis.com/auth/userinfo.email " +
                "https://www.googleapis.com/auth/userinfo.profile");
            var redirectUri = "https://localhost:7098/api/AuthorizeControllers/SignIn/";
            var url = "https://accounts.google.com/o/oauth2/auth?" +
                "approval_prompt=force" +
                "&scope=" + stringTestUrl +
                "&client_id=" + ClientId +
                "&redirect_uri=" + redirectUri +
                "&response_type=code" +
                "&access_type=offline"
                /*"&state=<userid>"*/;
            return RedirectPermanent(url);
        }


        /// <summary>
        /// SignIn
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [Route("SignIn")]
        [HttpGet]
        [Obsolete]
        public async Task<IActionResult> SignInAsync(string? code, string? scope, string? state = "")
        {
            var ClientId = _clientInfo.Client.client_id;
            var ClientSecret = _clientInfo.Client.client_secret;
            var googleInfo = new Root();
            var redirectUri = "https://localhost:7098/api/AuthorizeControllers/SignIn/";
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.google.com/o/oauth2/token?" +
                "grant_type=authorization_code" +
                "&code=" + code +
                "&client_id=" + ClientId +
                "&client_secret=" + ClientSecret +
                "&redirect_uri=" + redirectUri +
                "&scope=" + scope);
            var client = _clientFactory.CreateClient();
            var response = client.Send(request);
            TokenResponse token;
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var responseStream = response.Content.ReadAsStringAsync().Result;
                    token = JsonConvert.DeserializeObject<TokenResponse>(responseStream);
                }
                catch (Exception e)
                {
                    return NotFound(e.Message);
                }
            }
            else
            {
                return NotFound("Error occured!!");
            }

            //using (client = new HttpClient())
            //{
            //    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.AccessToken);
            //    using (response = await client.GetAsync("https://www.googleapis.com/userinfo/v2/me"))
            //    {

            //        string apiResponse = await response.Content.ReadAsStringAsync();
            //        Console.WriteLine(apiResponse);
            //    }
            //}

            var personFields = "ageRanges,addresses,biographies,birthdays,calendarUrls,clientData,coverPhotos,emailAddresses,events,externalIds,genders," +
                "imClients,interests,locales,locations,memberships,metadata,miscKeywords,names,nicknames,occupations," +
                "organizations,phoneNumbers,photos,relations,sipAddresses,skills,urls,userDefined";
            using (client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.AccessToken);
                using (response = await client.GetAsync("https://people.googleapis.com/v1/people/me?personFields=" +
                    personFields))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    // Root
                    googleInfo = JsonConvert.DeserializeObject<Root>(apiResponse);
                    Console.WriteLine(apiResponse);
                }
            }

            //using (client = new HttpClient())
            //{
            //    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.AccessToken);
            //    using (response = await client.GetAsync("https://people.googleapis.com/v1/people:searchContacts?query=&readMask=names,emailAddresses"))
            //    {

            //        string apiResponse = await response.Content.ReadAsStringAsync();
            //        Console.WriteLine(apiResponse);
            //    }
            //}
            using (client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.AccessToken);
                using (response = await client.GetAsync("https://people.googleapis.com/v1/people/me/connections?personFields=" +
                    personFields))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    // RootUserConnection
                    var rootUserConnection = JsonConvert.DeserializeObject<RootUserConnection>(apiResponse);
                    if (rootUserConnection != null && googleInfo != null)
                    {
                        googleInfo.rootUserConnection = rootUserConnection;
                    }
                    Console.WriteLine(apiResponse);
                }
            }

            return Ok(googleInfo);
        }

        /// <summary>
        /// LoginFacebook
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [Route("LoginFacebook")]
        [HttpGet]
        [Obsolete]
        public RedirectResult LoginFacebook()
        {
            string app_id = _clientInfoFB.Client.client_id;
            var scope = "email,public_profile,user_birthday,user_hometown,user_location,user_likes,user_photos,user_videos,user_friends,user_posts,user_gender,user_link,user_age_range";
            var result = "https://graph.facebook.com/oauth/authorize?client_id=" + app_id +
                "&redirect_uri=" + "https://localhost:7098/api/AuthorizeControllers/LoginFacebookSignIn/" +
                "&scope=" + scope;
            return RedirectPermanent(result);
        }

        /// <summary>
        /// LoginFacebookSignIn
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [Route("LoginFacebookSignIn")]
        [HttpGet]
        [Obsolete]
        public IActionResult LoginFacebookSignIn(string code)
        {
            string app_id = _clientInfoFB.Client.client_id;
            string app_secret = _clientInfoFB.Client.client_secret;
            var scope = "email,public_profile,user_birthday,user_hometown,user_location,user_likes,user_photos,user_videos,user_friends,user_posts,user_gender,user_link,user_age_range";
            FbAccessUser fb = new FbAccessUser();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.facebook.com/v3.2/oauth/access_token?client_id=" + app_id +
                "&redirect_uri=" + "https://localhost:7098/api/AuthorizeControllers/LoginFacebookSignIn/" +
                "&client_secret=" + app_secret +
                "&code=" + code +
                "&scope=" + scope);
            var client = _clientFactory.CreateClient();
            var response = client.Send(request);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var responseStream = response.Content.ReadAsStringAsync().Result;
                    fb = JsonConvert.DeserializeObject<FbAccessUser>(responseStream);
                }
                catch (Exception e)
                {
                    return NotFound(e.Message);
                }
            }
            Facebook.FacebookClient clientFb = new Facebook.FacebookClient(fb.access_token);

            var me = (IDictionary<string, object>)clientFb.Get("me" +
                "?fields=id,name,last_name,email,about,age_range,birthday,first_name,gender,hometown," +
                "link,location,languages,groups,education,favorite_athletes,favorite_teams,inspirational_people,install_type,installed,middle_name,name_format,website,videos");

            return Ok(me);
        }


        /// <summary>
        /// GetPersonInfoGoogle
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [Route("GetPersonInfoGoogle")]
        [HttpGet]
        [Obsolete]
        public async Task<IActionResult> GetPersonInfoGoogle(string accessToken)
        {
            var googleInfo = new Root();
            var personFields = "ageRanges,addresses,biographies,birthdays,calendarUrls,clientData,coverPhotos,emailAddresses,events,externalIds,genders," +
                "imClients,interests,locales,locations,memberships,metadata,miscKeywords,names,nicknames,occupations," +
                "organizations,phoneNumbers,photos,relations,sipAddresses,skills,urls,userDefined";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                using (var response = await client.GetAsync("https://people.googleapis.com/v1/people/me?personFields=" +
                    personFields))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    // Root
                    googleInfo = JsonConvert.DeserializeObject<Root>(apiResponse);
                    Console.WriteLine(apiResponse);
                }
            }


            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                using (var response = await client.GetAsync("https://people.googleapis.com/v1/people/me/connections?personFields=" +
                    personFields))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    // RootUserConnection
                    var rootUserConnection = JsonConvert.DeserializeObject<RootUserConnection>(apiResponse);
                    if (rootUserConnection != null && googleInfo != null)
                    {
                        googleInfo.rootUserConnection = rootUserConnection;
                    }
                    Console.WriteLine(apiResponse);
                }
            }

            try
            {
                return Ok(googleInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Ok();
            }
        }
        #endregion

        /// <summary>
        /// CRUD Create Data Provider
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [Authorize]
        [HttpPost]
        [Route("DataProviderCreate")]
        public async Task<IActionResult> DataProviderCreate(DataProviderRequest dataProvider)
        {
            var response = await _provider.POST(dataProvider);
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// CRUD Edit Data Provider
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [Authorize]
        [HttpPut]
        [Route("DataProviderEdit/{Id}")]
        public async Task<IActionResult> DataProviderEdit(Guid Id, DataProviderRequest dataProvider)
        {
            var response = await _provider.PUT(Id, dataProvider);
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// CRUD Get Data Provider by Id
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(DataProviderRequest))]
        [Authorize]
        [HttpGet]
        [Route("DataProviderGet")]
        public async Task<ActionResult<DataProviderRequest>> DataProviderGet(string token)
        {
            var response = await _provider.GETBYID(token);
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// CRUD Get Data Provider List
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<DataProviderRequest>))]
        [Authorize]
        [HttpGet]
        [Route("DataProviderGetList")]
        public async Task<ActionResult<List<DataProviderRequest>>> DataProviderGetList()
        {
            var response = await _provider.GETLIST();
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// CRUD Delete Data Provider by Id
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [Authorize]
        [HttpDelete]
        [Route("DataProviderDelete/{Id}")]
        public async Task<IActionResult> DataProviderDelete(Guid Id)
        {
            var response = await _provider.DELETE(Id);
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// CRUD Create Reward Category
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [Authorize]
        [HttpPost]
        [Route("RewardCategoryCreate")]
        public async Task<IActionResult> RewardCategoryCreate(RewardCategoryRequest rewardCategory)
        {
            var response = await _provider.RewardCategoryPOST(rewardCategory);
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// CRUD Edit Reward Category
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [Authorize]
        [HttpPut]
        [Route("RewardCategoryEdit/{Id}")]
        public async Task<IActionResult> RewardCategoryEdit(Guid Id, RewardCategoryRequest rewardCategory)
        {
            var response = await _provider.RewardCategoryPUT(Id, rewardCategory);
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// CRUD Get Reward Category by Id
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [AllowAnonymous]
        [HttpGet]
        [Route("RewardCategoryGet/{Id}")]
        public async Task<ActionResult<RewardCategoryRequest>> RewardCategoryGet(Guid Id)
        {
            var response = await _provider.RewardCategoryGETBYID(Id);
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// CRUD Get Reward Category List
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [AllowAnonymous]
        [HttpGet]
        [Route("RewardCategoryGetList")]
        public async Task<ActionResult<List<RewardCategoryRequest>>> RewardCategoryGetList()
        {
            var response = await _provider.RewardCategoryGETLIST();
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// CRUD Delete Reward Category by Id
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [Authorize]
        [HttpDelete]
        [Route("RewardCategoryDelete/{Id}")]
        public async Task<IActionResult> RewardCategoryDelete(Guid Id)
        {
            var response = await _provider.RewardCategoryDELETE(Id);
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }
        /// <summary>
        /// CRUD Create Transaction
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [AllowAnonymous]
        [HttpPost]
        [Route("TransactionCreate")]
        public async Task<IActionResult> TransactionCreate(TransactionRequest Transaction)
        {
            var response = await _provider.TransactionPOST(Transaction);
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// CRUD Edit Transaction
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [AllowAnonymous]
        [HttpPut]
        [Route("TransactionEdit/{Id}")]
        public async Task<IActionResult> TransactionEdit(Guid Id, TransactionRequest Transaction)
        {
            var response = await _provider.TransactionPUT(Id, Transaction);
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// CRUD Get Transaction by Id
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(TransactionRequest))]
        [AllowAnonymous]
        [HttpGet]
        [Route("TransactionGet/{Id}")]
        public async Task<ActionResult<TransactionRequest>> TransactionGet(Guid Id)
        {
            var response = await _provider.TransactionGETBYID(Id);
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// CRUD Get Transaction List
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<TransactionRequest>))]
        //[Authorize]
        [HttpGet]
        [Route("TransactionGetList")]
        public async Task<ActionResult<List<TransactionRequest>>> TransactionGetList()
        {
            var response = await _provider.TransactionGETLIST();
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// CRUD Get Country List
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<CountryRequest>))]
        [Authorize]
        [HttpGet]
        [Route("GETLISTCountry")]
        public async Task<ActionResult<List<CountryRequest>>> Country()
        {
            var response = await _provider.GETLISTCountry();
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// CRUD Delete Transaction by Id
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [Authorize]
        [HttpDelete]
        [Route("TransactionDelete/{Id}")]
        public async Task<IActionResult> TransactionDelete(Guid Id)
        {
            var response = await _provider.TransactionDELETE(Id);
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// Get the terms of use status for user from provider
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<TransactionRequest>))]
        //[Authorize]
        [HttpGet]
        [Route("TermsOfUse")]
        public async Task<ActionResult<List<TransactionRequest>>> TermOfUseStatus(string fio, Guid userId, Guid provaiderId)
        {
            var response = await _provider.TermOfUseStatus(fio, userId, provaiderId);
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// Get extended status for user from provider's terms of use
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<TransactionRequest>))]
        //[Authorize]
        [HttpPost]
        [Route("TermsOfUseExtended")]
        public async Task<ActionResult<List<TransactionRequest>>> TermOfUseStatusExtended([FromBody] TermOfUseRequest model)
        {
            var response = await _provider.TermOfUseStatus(model.userId, model.provaiderId, model.email, model.phone);
            if (response == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// Agree to the terms of use
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<TransactionRequest>))]
        [Authorize]
        [HttpGet]
        [Route("TermOfUseApply")]
        public async Task<ActionResult<List<TransactionRequest>>> TermOfUseApply(Guid userId, Guid providerId)
        {
            var response = await _provider.TermOfUseApply(userId, providerId);
            if (response == false)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }

        /// <summary>
        /// Refuse terms of services with provider
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<TransactionRequest>))]
        [Authorize]
        [HttpGet]
        [Route("TermOfUseCancel")]
        public async Task<ActionResult<List<TransactionRequest>>> TermOfUseCancel(Guid userId, Guid providerId)
        {
            var response = await _provider.TermOfUseCancel(userId, providerId);
            if (response == false)
            {
                return BadRequest();
            }
            else
            {
                return Ok(response);
            }
        }



        /// <summary>
        /// Get statistics for user extended
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(AllDataFromStatisticRequest))]
        [Authorize]
        [HttpGet]
        [Route("GetStatisticsExtend")]
        public async Task<ActionResult<AllDataFromStatisticRequest>> GetStatisticsExtend(string userId)
                            => await _provider.GetStatisticsExtend(userId);

        /// <summary>
        /// Upload logo
        /// </summary>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GeneralResponse))]
        [Authorize]
        [HttpPost("upload_logo")]
        public async Task<IActionResult> UploadAsync([FromBody] Uploadrequest providerLogoModel)
        {
            try
            {
                var result = await _provider.Upload(providerLogoModel);

                if (result.Code == 200)
                {
                    return Ok(result.Message);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        /// <summary>
        /// Get User info for provider
        /// </summary>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GeneralResponse))]
        [Authorize]
        [HttpPost("get_user_info")]
        public async Task<IActionResult> GetUserInfo([FromBody] UserInfoModel model)
        {
            try
            {
                var result = await _provider.GetUserInfo(model);

                if (result.Code == 200)
                {
                    return Ok(result.Message);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}

