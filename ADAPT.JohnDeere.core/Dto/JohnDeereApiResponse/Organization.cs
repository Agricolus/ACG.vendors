using System;
using Newtonsoft.Json;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    public class Organization
    {
        [JsonProperty("@type")]
        public string OType { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Member { get; set; }
        public bool Internal { get; set; }
        public int Id { get; set; }
        public Link[] Links { get; set; }
    }
}
