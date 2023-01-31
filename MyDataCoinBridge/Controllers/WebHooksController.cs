using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyDataCoinBridge.Interfaces;
using MyDataCoinBridge.Models;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using MyDataCoinBridge.Services;

namespace MyDataCoinBridge.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WebHooksController: ControllerBase
	{
        private readonly ILogger<UserController> _logger;
        private readonly IWebHooks _hookService;

        public WebHooksController(ILogger<UserController> logger, IWebHooks hookService)
        {
            _hookService = hookService;
            _logger = logger;
        }

        /// <summary>
        /// Create Web Hook
        /// </summary>
        /// <response code="200">Returns Ok</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GeneralResponse))]
        [AllowAnonymous]
        [HttpGet]
        [Route("subscribe")]
        public async Task<IActionResult> SendCode(string email)
        {
            GeneralResponse response = await _userService.SendCode(email);

            if (response.Code == 400)
                return BadRequest(response.Message);
            else
                return Ok(response);
        }
    }
}

