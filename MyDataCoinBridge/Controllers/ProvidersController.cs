using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyDataCoinBridge.Entities;
using MyDataCoinBridge.Interfaces;
using MyDataCoinBridge.Models;
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
        [Route("get-users-providers/{countryCode}")]
        public async Task<IActionResult> GetUsersProviders(string countryCode)
        {
            var result = await _provider.GetUsersProvidersAsync(countryCode);
            return Ok(result);
        }



        /// <summary>
        /// LoginGoogle
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="421">Returns User Not Found</response>
        /// <response code="500">Returns Internal Server Error</response>
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
                return NotFound("������ �����������!!");
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
                return Ok();
            }
        }

        #region OLD_MASHINA_KG
        ///// <summary>
        ///// GetPersonInfoFB
        ///// </summary>
        ///// <response code="200">Returns Success</response>
        ///// <response code="400">Returns Bad Request</response>
        ///// <response code="401">Returns Unauthorized</response>
        ///// <response code="415">Returns Unsupported Media Type</response>
        ///// <response code="421">Returns User Not Found</response>
        ///// <response code="500">Returns Internal Server Error</response>
        //[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        //[Authorize]
        //[Route("GetPersonInfoFB")]
        //[HttpGet]
        //public async Task<IActionResult> GetPersonInfoFB(string accessToken)
        //{
        //    Facebook.FacebookClient clientFb = new Facebook.FacebookClient(accessToken);

        //    var me = (IDictionary<string, object>)clientFb.Get("me" +
        //        "?fields=id,name,last_name,email,about,age_range,birthday,first_name,gender,hometown," +
        //        "link,location,languages,groups,education,favorite_athletes,favorite_teams,inspirational_people,install_type,installed,middle_name,name_format,website,videos");

        //    return Ok(me);
        //}

        ///// <summary>
        ///// GetPersonInfoMashinaKG
        ///// </summary>
        ///// <response code="200">Returns Success</response>
        ///// <response code="400">Returns Bad Request</response>
        ///// <response code="401">Returns Unauthorized</response>
        ///// <response code="415">Returns Unsupported Media Type</response>
        ///// <response code="421">Returns User Not Found</response>
        ///// <response code="500">Returns Internal Server Error</response>
        //[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        //[Authorize]
        //[Route("GetPersonInfoMashinaKG")]
        //[HttpGet]
        //public async Task<IActionResult> GetPersonInfoMashinaKG(string gmail)
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Get, "https://dmc.mashine.kg/v1/public/user/info/" + gmail);
        //    var client = _clientFactory.CreateClient();
        //    client.DefaultRequestHeaders.Add("dmc-auth", "Bearer nMeTRAlYthRiZErIDEcHrImeNcONGunDRUtAteNTOwpUStiGnOMerWaleymPOloT");
        //    var response = client.Send(request);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        try
        //        {
        //            return Ok(response.Content.ReadAsStringAsync().Result);
        //        }
        //        catch (Exception e)
        //        {
        //            return NotFound(e.Message);
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }
        //}

        ///// <summary>
        ///// GetTransactionMashinaKG
        ///// </summary>
        ///// <response code="200">Returns Success</response>
        ///// <response code="400">Returns Bad Request</response>
        ///// <response code="401">Returns Unauthorized</response>
        ///// <response code="415">Returns Unsupported Media Type</response>
        ///// <response code="421">Returns User Not Found</response>
        ///// <response code="500">Returns Internal Server Error</response>
        //[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        //[Authorize]
        //[Route("GetTransactionMashinaKG")]
        //[HttpGet]
        //public async Task<IActionResult> GetTransactionMashinaKG(string gmail)
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Get, "https://dmc.mashine.kg/v1/public/user/transactions/" + gmail);
        //    var client = _clientFactory.CreateClient();
        //    client.DefaultRequestHeaders.Add("dmc-auth", "Bearer nMeTRAlYthRiZErIDEcHrImeNcONGunDRUtAteNTOwpUStiGnOMerWaleymPOloT");
        //    var response = client.Send(request);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        try
        //        {
        //            return Ok(response.Content.ReadAsStringAsync().Result);
        //        }
        //        catch (Exception e)
        //        {
        //            return NotFound(e.Message);
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }
        //}

        ///// <summary>
        ///// GetStatsInfoMashinaKG
        ///// </summary>
        ///// <response code="200">Returns Success</response>
        ///// <response code="400">Returns Bad Request</response>
        ///// <response code="401">Returns Unauthorized</response>
        ///// <response code="415">Returns Unsupported Media Type</response>
        ///// <response code="421">Returns User Not Found</response>
        ///// <response code="500">Returns Internal Server Error</response>
        //[SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        //[Authorize]
        //[Route("GetStatsInfoMashinaKG")]
        //[HttpGet]
        //public async Task<IActionResult> GetStatsInfoMashinaKG(string gmail)
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Get, "https://dmc.mashine.kg/v1/public/user/stats/" + gmail);
        //    var client = _clientFactory.CreateClient();
        //    client.DefaultRequestHeaders.Add("dmc-auth", "Bearer nMeTRAlYthRiZErIDEcHrImeNcONGunDRUtAteNTOwpUStiGnOMerWaleymPOloT");
        //    var response = client.Send(request);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        try
        //        {
        //            return Ok(response.Content.ReadAsStringAsync().Result);
        //        }
        //        catch (Exception e)
        //        {
        //            return NotFound(e.Message);
        //        }
        //    }
        //    else
        //    {
        //        return BadRequest();
        //    }
        //}
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
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [Authorize]
        [HttpGet]
        [Route("DataProviderGet/{Id}")]
        public async Task<ActionResult<DataProviderRequest>> DataProviderGet(Guid Id)
        {
            var response = await _provider.GETBYID(Id);
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
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
        [Authorize]
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
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Models.GeneralResponse))]
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
    }
}

