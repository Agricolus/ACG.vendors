using Newtonsoft.Json;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    public class Area
    {
        [JsonProperty("@type")]
        public string OType { get; set; }
        public double ValueAsDouble { get; set; }
        public string Unit { get; set; }
    }
}