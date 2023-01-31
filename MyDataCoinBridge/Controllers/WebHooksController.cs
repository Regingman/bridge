﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyDataCoinBridge.Interfaces;
using MyDataCoinBridge.Models;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using MyDataCoinBridge.Services;
using MyDataCoinBridge.Models.WebHooks;

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
        /// Subscribe
        /// </summary>
        /// <response code="200">Returns Ok</response>
        /// <response code="204">Returns User Not Found</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GeneralResponse))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Type = typeof(NoContentResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(GeneralResponse))]
        [SwaggerResponse((int)HttpStatusCode.UnsupportedMediaType, Type = typeof(UnsupportedMediaTypeResult))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(GeneralResponse))]
        [AllowAnonymous]
        [HttpPost]
        [Route("event")]
        public async Task<IActionResult> Event(WebHookUserProfileModel.Profile model)
        {
            string response = await _hookService.Event(model);

            //if (response.Code == 400)
            //    return BadRequest(response);
            //else if (response.Code == 204)
            //    return NoContent();
            //else
                return Ok(response);
        }

        /// <summary>
        /// Subscribe
        /// </summary>
        /// <response code="200">Returns Ok</response>
        /// <response code="204">Returns User Not Found</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GeneralResponse))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Type = typeof(NoContentResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(GeneralResponse))]
        [SwaggerResponse((int)HttpStatusCode.UnsupportedMediaType, Type = typeof(UnsupportedMediaTypeResult))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(GeneralResponse))]
        [AllowAnonymous]
        [HttpPost]
        [Route("subscribe")]
        public async Task<IActionResult> Subscribe(SubscribeRequest model)
        {
            GeneralResponse response = await _hookService.Subscribe(model);

            if (response.Code == 400)
                return BadRequest(response);
            else if (response.Code == 204)
                return NoContent();
            else
                return Ok(response);
        }

        /// <summary>
        /// Unsubscribe
        /// </summary>
        /// <response code="200">Returns Ok</response>
        /// <response code="204">Returns User Not Found</response>
        /// <response code="400">Returns Bad Request</response>
        /// <response code="415">Returns Unsupported Media Type</response>
        /// <response code="500">Returns Internal Server Error</response>
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GeneralResponse))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Type = typeof(NoContentResult))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(GeneralResponse))]
        [SwaggerResponse((int)HttpStatusCode.UnsupportedMediaType, Type = typeof(UnsupportedMediaTypeResult))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(GeneralResponse))]
        [AllowAnonymous]
        [HttpGet]
        [Route("unsubscribe/{token}")]
        public async Task<IActionResult> Unsubscrube(string token)
        {
            GeneralResponse response = await _hookService.Unsubscribe(token);

            if (response.Code == 400)
                return BadRequest(response);
            else if (response.Code == 204)
                return NoContent();
            else
                return Ok(response);
        }
    }
}
