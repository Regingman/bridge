using System;
using MyDataCoinBridge.Enums;
using MyDataCoinBridge.Entities;

namespace MyDataCoinBridge.Models.WebHooks
{
	public class UserPrivacyProfileModel
    {
        public Profile Profile { get; set; }
        public BasicData BasicData { get; set; }
        public Contacts Contacts { get; set; }
        public WorkAndEducation WorkAndEducation { get; set; }
        public PlaceOfResidence PlaceOfResidence { get; set; }
        public PersonalInterests PersonalInterests { get; set; }
        public UserPrivacySetting UserPrivacySettings { get; set; }
    }

    public class Profile
    {
        public string LabelName_en = "Profile";
        public string LabelName_ru = "Профиль";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; } // 0-Male, 1-Female
        public string[] Email { get; set; }
        public string[] Phone { get; set; }
    }

    public class BasicData
    {
        public string LabelName_en = "Basic Data";
        public string LabelName_ru = "Базовая инфрмация";
        public string[] Interests { get; set; } = new string[] {};
        public string[] Languages { get; set; } = new string[] {};
        public string[] ReligionViews { get; set; } = new string[] {};
        public string[] PoliticalViews { get; set; } = new string[] {};

    }

    public class Contacts
    {
        public string LabelName_en = "Contacts";
        public string LabelName_ru = "Контакты";
        public string MobilePhone { get; set; }
        public string Address { get; set; }
        public string[] LinkedAccounts { get; set; } = new string[] {};
        public string Website { get; set; }
    }

    public class WorkAndEducation
    {
        public string LabelName_en = "Work and education";
        public string LabelName_ru = "Работа и образование";
        public string PlaceOfWork { get; set; }
        public string[] Skills { get; set; } = new string[] {};
        public string University { get; set; }
        public string Faculty { get; set; }
    }

    public class PlaceOfResidence
    {
        public string LabelName_en = "Place of residence";
        public string LabelName_ru = "Место проживания";
        public string CurrentCity { get; set; }
        public string BirthPlace { get; set; }
        public string[] OtherCities { get; set; } = new string[] {};
    }

    public class PersonalInterests
    {
        public string LabelName_en = "Personal Interests";
        public string LabelName_ru = "Личные интересы";
        public string BreifDescription { get; set; }
        public string[] Hobby { get; set; } = new string[] {};
        public string[] Sport { get; set; } = new string[] {};
    }
}

