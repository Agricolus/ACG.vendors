using Newtonsoft.Json;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    public class Polygon
    {
        [JsonProperty("@type")]
        public string OType { get; set; }
        public Ring[] Rings { get; set; }
    }
}