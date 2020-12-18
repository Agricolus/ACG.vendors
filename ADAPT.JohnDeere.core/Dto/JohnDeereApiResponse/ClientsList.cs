using System;
using Newtonsoft.Json;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    public class ClientsList
    {
        [JsonProperty("@type")]
        public string OType { get; set; }
        public Client[] Clients { get; set; }
        public object OtherAttributes { get; set; }
    }
}
