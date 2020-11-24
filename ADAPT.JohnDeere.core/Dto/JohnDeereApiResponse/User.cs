using System;
using Newtonsoft.Json;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    public class User
    {
        [JsonProperty("@type")]
        public string OType { get; set; }

        public string AccountName { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string UserType { get; set; }

        public Link[] Links { get; set; }

    }
}
