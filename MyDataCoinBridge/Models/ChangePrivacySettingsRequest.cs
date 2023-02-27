using System;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoinBridge.Models.WebHooks
{
	public class ChangePrivacySettingsRequest
	{
        /// <summary>
        /// Users email
        /// <example>example@mail.com</example>
        /// </summary>
        [Required]
		public string Email { get; set; }

        /// <summary>
        /// Category
        /// <example>BasicData</example>
        /// </summary>
		[Required]
        public string Category { get; set; }

        /// <summary>
        /// The value
        /// <example>true</example>
        /// </summary>
        public bool SettingValue { get; set; }
	}
}