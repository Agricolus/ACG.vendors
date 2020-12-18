using System;
using Newtonsoft.Json;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    public class ReportedLocation
    {
        [JsonProperty("@type")]
        public string OType { get; set; }
        
        public Point Point { get; set; }

        public DateTime EventTimestamp { get; set; }

        public DateTime GpsFixTimestamp { get; set; }
        
        public Link[] Links { get; set; }

    }
}
