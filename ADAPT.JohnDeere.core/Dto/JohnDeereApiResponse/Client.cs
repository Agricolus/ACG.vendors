using System;
using Newtonsoft.Json;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    public class Client
    {
        [JsonProperty("@type")]
        public string OType { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
        public Link[] Links { get; set; }
        
    }
}
