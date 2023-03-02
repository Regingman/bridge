using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyDataCoinBridge.Interfaces;
using MyDataCoinBridge.Models;
using MyDataCoinBridge.Models.Transaction;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace MyDataCoinBridge.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BridgeStatisticController : ControllerBase
    {
        private readonly ILogger<ProvidersController> _logger;
        private readonly IProviders _provider;

        public BridgeStatisticController(ILogger<ProvidersController> logger, IProviders provider)
        {
            _logger = logger;
            _provider = provider;
        }

        /// <summary>
        /// add statistic from provider
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GeneralResponse))]
        [AllowAnonymous]
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> TransactionAddProvider(string token, [FromBody] List<TransactionProviderRequest> model)
        {
            var result = await _provider.TransactionAddProvider(token, model);
            if (result.Code == 400)
                return BadRequest(result.Message);
            else
                return Ok(result);
        }

        /// <summary>
        /// get statistic from provider
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<TransactionProviderResponse>))]
        [AllowAnonymous]
        [HttpGet]
        [Route("getFromProvider")]
        public async Task<IActionResult> GetStatisticFromProvider(string token)
        {
            var result = await _provider.GetStatisticFromProvider(token);
            return Ok(result);
        }

        /// <summary>
        /// get statistic from admin 
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<TransactionProviderResponse>))]
        [AllowAnonymous]
        [HttpGet]
        [Route("getFromAdmin")]
        public async Task<IActionResult> GetStatisticFromProviderFromAdmin()
        {
            var result = await _provider.GetStatisticFromProviderFromAdmin();
            return Ok(result);
        }

        /// <summary>
        /// claim USDMDC from user
        /// </summary>
        /// <response code="200">Returns Success</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="401">Returns Unauthorized</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(decimal))]
        [AllowAnonymous]
        [HttpGet]
        [Route("claim/{userId}")]
        public async Task<IActionResult> GetClaimStatistic(string userId)
        {
            var result = await _provider.Claim(userId);
            return Ok(result);
        }
    }
}
