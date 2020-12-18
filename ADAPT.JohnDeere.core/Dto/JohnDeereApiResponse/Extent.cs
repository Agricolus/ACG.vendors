using Newtonsoft.Json;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    public class Extent
    {
        [JsonProperty("@type")]
        public string OType { get; set; }
        public Point TopLeft { get; set; }
        public Point BottomRight { get; set; }
    }
}