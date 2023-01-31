using System;
using MyDataCoinBridge.Enums;

namespace MyDataCoinBridge.Models.Events
{
    public class WebHookCreated : DomainEvent
    {

        public WebHookCreated() { }

        public long WebHookId { get; set; }

        // Add any custom props hire...
    }

    public class DomainEvent
    {
        public long ID { get; set; }
#nullable enable
        public Guid? ActorID { get; set; }
#nullable disable
        public DateTime TimeStamp { get; set; }

        public EventType EventType { get; set; }
    }
}

