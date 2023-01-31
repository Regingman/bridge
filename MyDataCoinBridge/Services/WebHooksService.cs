﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyDataCoinBridge.DataAccess;
using MyDataCoinBridge.Entities;
using MyDataCoinBridge.Enums;
using MyDataCoinBridge.Interfaces;
using MyDataCoinBridge.Models;
using MyDataCoinBridge.Models.WebHooks;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace MyDataCoinBridge.Services
{
	public class WebHooksService: IWebHooks
	{
        private readonly WebApiDbContext _context;
        private readonly ILogger<UserService> _logger;

        public WebHooksService(ILogger<UserService> logger, WebApiDbContext context)
		{
            _logger = logger;
            _context = context;
        }

        public async Task<string> Event(WebHookUserProfileModel.Profile message)
        {
            _logger.LogInformation($"Hello, {message}");
            return new string($"Hello, {message}");
        }

        public async Task<GeneralResponse> Subscribe(SubscribeRequest model)
        {
            try
            {
                var provider = _context.BridgeUsers.
                    SingleOrDefaultAsync(
                    x => x.TokenForService == model.Token &&
                    x.IsVerified == VerifiedEnum.Yes);

                if (provider == null)
                    return new GeneralResponse(204, "User Not Found");

                WebHook hook = new WebHook();

                hook.ID = Guid.NewGuid();
                hook.IsActive = true;
                hook.ContentType = "application/json";
                hook.Secret = model.Token;
                hook.WebHookUrl = model.URL;
                hook.HookEvents = new HookEventType[]
                {
                    HookEventType.pd_requested,
                    HookEventType.report_requested
                };

                await _context.WebHooks.AddAsync(hook);
                await _context.SaveChangesAsync();

                return new GeneralResponse(200, "Ok");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResponse(400, ex.Message);
            }
        }

        public async Task<GeneralResponse> Unsubscribe(string token)
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema schema = generator.Generate(typeof(WebHookUserProfileModel.Profile));
            Console.WriteLine(schema.ToString());
            return new GeneralResponse(200, schema.ToString());

            try
            {
                var provider = _context.BridgeUsers.
                    SingleOrDefaultAsync(
                    x => x.TokenForService == token);

                if (provider == null)
                    return new GeneralResponse(204, "User Not Found");

                var res = await _context.WebHooks.SingleOrDefaultAsync(x => x.Secret == token);
                res.IsActive = false;
                return new GeneralResponse(200, "Ok");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResponse(400, ex.Message);
            }
        }
    }
}
