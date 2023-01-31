using System;
namespace MyDataCoinBridge.Entities
{
    public class WebHookHeader
    {

        /// <summary>
        /// Hook header ID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Linked Webhook Id
        /// </summary>
        public Guid WebHookID { get; set; }

        /// <summary>
        /// Linked Webhook
        /// </summary>
        public WebHook WebHook { get; set; }

        /// <summary>
        /// Header Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Header content
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Header created time
        /// </summary>
        public DateTime CreatedTimestamp { get; set; }
    }
}

