using System;
namespace MyDataCoinBridge.Models.WebHooks
{
	public class WebHookUserProfileModel
    {
		public class Profile
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public int? Gender { get; set; } // 0-Male, 1-Female
            public string[] Email { get; set; }
            public string[] Phone { get; set; }
            public BasicData BasicData { get; set; }
            public Contacts Contacts { get; set; }
            public WorkAndEducation WorkAndEducation { get; set; }
            public PlaceOfResidence PlaceOfResidence { get; set; }
            public PersonalInterests PersonalInterests { get; set; }
        }

        public class BasicData
        {
            public string[] Interests { get; set; }
            public string[] Languages { get; set; }
            public string[] ReligionViews { get; set; }
            public string[] PoliticalViews { get; set; }

        }

        public class Contacts
        {
            public string MobilePhone { get; set; }
            public string Address { get; set; }
            public string[] LinkedAccounts { get; set; }
            public string Website { get; set; }
        }

        public class WorkAndEducation
        {
            public string PlaceOfWork { get; set; }
            public string[] Skills { get; set; }
            public string University { get; set; }
            public string Faculty { get; set; }
        }

        public class PlaceOfResidence
        {
            public string CurrentCity { get; set; }
            public string BirthPlace { get; set; }
            public string[] OtherCities { get; set; }
        }

        public class PersonalInterests
        {
            public string BreifDescription { get; set; }
            public string[] Hobby { get; set; }
            public string[] Sport { get; set; }
        }
    }
}

