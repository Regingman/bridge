using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyDataCoinBridge.Interfaces;
using MyDataCoinBridge.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace MyDataCoinBridge.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUser _userService;

        public UserController(ILogger<UserController> logger, IUser userService)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// User authentication by email, needs to authenticate managers
        /// </summary>
        /// <response code="200">Returns Ok</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(AuthenticateResponse))]
        [AllowAnonymous]
        [HttpGet]
        [Route("send_code/{email}")]
        public async Task<IActionResult> SendCode(string email)
        {
            GeneralResponse response = await _userService.SendCode(email);

            if (response.Code == 400)
                return BadRequest(response.Message);
            else
                return Ok(response);
        }

        /// <summary>
        /// Verify code sent by system to get JWT
        /// </summary>
        /// <response code="200">Returns Ok</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(VerifyCodeResponse))]
        [AllowAnonymous]
        [HttpPost]
        [Route("verify_code")]
        public async Task<VerifyCodeResponse> VerifyCode([FromBody] VerifyCodeRequest request)
        {
            return await _userService.VerifyCode(request);
        }

        /// <summary>
        /// Verify code sent by system to get JWT
        /// </summary>
        /// <response code="200">Returns Ok</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(VerifyCodeResponse))]
        [AllowAnonymous]
        [HttpPost]
        [Route("registration/{email}")]
        public async Task<IActionResult> Register(string email)
        {
            GeneralResponse response = await _userService.Registration(email);

            if (response.Code == 400)
                return BadRequest(response.Message);
            else
                return Ok(response);
        }
    }
}
