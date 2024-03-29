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

namespace MyDataCoinBridge.Services
{
    public class WebHooksService : IWebHooks
    {
        private readonly WebApiDbContext _context;
        private readonly ILogger<UserService> _logger;

        public WebHooksService(ILogger<UserService> logger, WebApiDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<GeneralResponse> EditUrl(SubscribeRequest model)
        {
            try
            {
                var webhook = await _context.WebHooks.SingleOrDefaultAsync(x => x.Secret == model.Token);
                
                webhook.IsActive = true;
                webhook.ContentType = "application/json";
                webhook.WebHookUrl = model.URL;
                webhook.HookEvents = new HookEventType[]
                {
                    HookEventType.pd_requested,
                    HookEventType.report_requested
                };

                _context.WebHooks.Update(webhook);
                await _context.SaveChangesAsync();
                return new GeneralResponse(200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResponse(400, "Error!");
            }
        }

        public async Task<GeneralResponse> GetUrl(string token)
        {
            var res = await _context.WebHooks.SingleOrDefaultAsync(x => x.Secret == token);
            if (res == null)
                return new GeneralResponse(204, "Not Found");
            else
                if (!res.IsActive)
            {
                return new GeneralResponse(400, "Unsubscribe!");
            }
            else
                return new GeneralResponse(200, res.WebHookUrl);
        }

        public async Task<GeneralResponse> Subscribe(SubscribeRequest model)
        {
            try
            {
                var provider = await _context.BridgeUsers.
                    SingleOrDefaultAsync(
                    x => x.TokenForService == model.Token &&
                    x.IsVerified == VerifiedEnum.Yes);

                if (provider == null)
                    return new GeneralResponse(204, "User Not Found");

                var webhook = await _context.WebHooks.SingleOrDefaultAsync(x => x.Secret == model.Token);

                if (webhook.IsActive)
                    return new GeneralResponse(200, "Already Subscribed");

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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResponse(400, ex.Message);
            }
        }

        public async Task<GeneralResponse> Unsubscribe(string token)
        {
            try
            {
                var provider = await _context.BridgeUsers.
                    SingleOrDefaultAsync(
                    x => x.TokenForService == token);

                if (provider == null)
                    return new GeneralResponse(204, "User Not Found");

                var res = await _context.WebHooks.SingleOrDefaultAsync(x => x.Secret == token);
                res.IsActive = false;
                _context.WebHooks.Update(res);
                await _context.SaveChangesAsync();
                return new GeneralResponse(200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new GeneralResponse(400, ex.Message);
            }
        }
    }
}

