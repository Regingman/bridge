using System;
namespace MyDataCoinBridge.Enums
{
    // Actions of HookEventType
    public enum HookResourceAction
    {
        undefined,
        hook_created,
        hook_removed,
        hook_updated,
        // etc...
    }

    // Actions of PDRequestEventType
    public enum DataRequestAction
    {
        undefined,
        pd_requested,
        //etc...
    }

    public enum ReportRequest
    {
        undefined,
        report_requested,
        //etc...
    }
}

