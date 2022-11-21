using System;
using System.Collections.Generic;

namespace MyDataCoinBridge.Models
{
    public class GoogleMainModel
    {
        public class AgeRange
        {
            public Metadata metadata { get; set; }
            public string ageRange { get; set; }
        }

        public class Birthday
        {
            public Metadata metadata { get; set; }
            public Date date { get; set; }
        }

        public class CoverPhoto
        {
            public Metadata metadata { get; set; }
            public string url { get; set; }
            public bool @default { get; set; }
        }

        public class Date
        {
            public int year { get; set; }
            public int month { get; set; }
            public int day { get; set; }
        }

        public class EmailAddress
        {
            public Metadata metadata { get; set; }
            public string value { get; set; }
        }

        public class Gender
        {
            public Metadata metadata { get; set; }
            public string value { get; set; }
            public string formattedValue { get; set; }
        }

        public class Locale
        {
            public Metadata metadata { get; set; }
            public string value { get; set; }
        }

        public class Metadata
        {
            public List<Source> sources { get; set; }
            public string objectType { get; set; }
            public bool primary { get; set; }
            public Source source { get; set; }
            public bool sourcePrimary { get; set; }
            public bool verified { get; set; }
        }

        public class ProfileMetadata
        {
            public string objectType { get; set; }
            public List<string> userTypes { get; set; }
        }

        public class Root
        {
            public string resourceName { get; set; }
            public string etag { get; set; }
            public Metadata metadata { get; set; }
            public List<Locale> locales { get; set; }
            public List<Name> names { get; set; }
            public List<CoverPhoto> coverPhotos { get; set; }
            public List<Photo> photos { get; set; }
            public List<Gender> genders { get; set; }
            public List<Birthday> birthdays { get; set; }
            public List<EmailAddress> emailAddresses { get; set; }
            public List<AgeRange> ageRanges { get; set; }
            public RootUserConnection rootUserConnection { get; set; }
        }

        public class Source
        {
            public string type { get; set; }
            public string id { get; set; }
            public string etag { get; set; }
            public ProfileMetadata profileMetadata { get; set; }
            public DateTime updateTime { get; set; }
        }

        public class Source2
        {
            public string type { get; set; }
            public string id { get; set; }
        }

        public class Connection
        {
            public string resourceName { get; set; }
            public string etag { get; set; }
            public Metadata metadata { get; set; }
            public List<Name> names { get; set; }
            public List<Photo> photos { get; set; }
            public List<PhoneNumber> phoneNumbers { get; set; }
            public List<Membership> memberships { get; set; }
        }

        public class ContactGroupMembership
        {
            public string contactGroupId { get; set; }
            public string contactGroupResourceName { get; set; }
        }

        public class Membership
        {
            public Metadata metadata { get; set; }
            public ContactGroupMembership contactGroupMembership { get; set; }
        }

        public class Name
        {
            public Metadata metadata { get; set; }
            public string displayName { get; set; }
            public string familyName { get; set; }
            public string givenName { get; set; }
            public string displayNameLastFirst { get; set; }
            public string unstructuredName { get; set; }
        }

        public class PhoneNumber
        {
            public Metadata metadata { get; set; }
            public string value { get; set; }
            public string canonicalForm { get; set; }
            public string type { get; set; }
            public string formattedType { get; set; }
        }

        public class Photo
        {
            public Metadata metadata { get; set; }
            public string url { get; set; }
            public bool @default { get; set; }
        }

        public class RootUserConnection
        {
            public List<Connection> connections { get; set; }
            public int totalPeople { get; set; }
            public int totalItems { get; set; }
        }
    }
}
