using System;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoinBridge.Models.WebHooks
{
	public class SubscribeRequest
	{
        /// <summary>
        /// Access Token from adminpanel
        /// <example>$2a$11$345F#lBpXelQlWknng4JGeU4c.UnjEZii#%$g#wMnLwvAey.dd</example>
        /// </summary>
        [Required]
		public string Token { get; set; }

        /// <summary>
        /// URL of your service which will return personal data
        /// <example>https://yourwebsite/api/v1/pd_request</example>
        /// </summary>
		[Required]
        [Url]
        public string URL { get; set; }
	}
}
