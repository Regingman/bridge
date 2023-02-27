using System;
using System.ComponentModel.DataAnnotations;

namespace MyDataCoinBridge.Entities
{
    public class UserPrivacySetting
	{
        [Key]
        public Guid Id { get; set; }

        public string Email { get; set; }

		public bool Profile { get; set; } = true;

        public bool BasicData { get; set; } = false;

        public bool Contacts { get; set; } = false;

        public bool WorkAndEducation { get; set; } = false;

        public bool PlaceOfResidence { get; set; } = false;

        public bool PersonalInterests { get; set; } = false;
    }
}

