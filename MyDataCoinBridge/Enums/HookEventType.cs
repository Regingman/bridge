using System;
namespace MyDataCoinBridge.Enums
{
    public enum HookEventType
    {
        hook, //(Hook created, Hook deleted ...)

        pd_requested, // (Personal data requested)

        report_requested // (Report requested)
    }
}

