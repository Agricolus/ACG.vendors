using Newtonsoft.Json;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    public class Link
    {
        [JsonProperty("@type")]
        public string OType { get; set; }
        public string Rel { get; set; }
        public string Uri { get; set; }
    }
}
