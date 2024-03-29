﻿using MyDataCoinBridge.Entities;

namespace MyDataCoinBridge.Models
{
    public class AuthBody
    {
        public AuthBody()
        {
        }

        public AuthBody(BridgeUser user, Tokens tokens)
        {
            User = user;
            Tokens = tokens;
        }

        public BridgeUser User { get; set; }

        public Tokens Tokens { get; set; }
    }

    public class AuthenticateResponse
    {
        public AuthenticateResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public AuthenticateResponse(int code, string message, AuthBody body)
        {
            Code = code;
            Message = message;
            BodyResponse = body;
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public AuthBody BodyResponse { get; set; }
    }
}
