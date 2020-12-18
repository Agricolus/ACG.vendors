using System;
using Newtonsoft.Json;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    public class Point
    {
        [JsonProperty("@type")]
        public string OType { get; set; }

        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
