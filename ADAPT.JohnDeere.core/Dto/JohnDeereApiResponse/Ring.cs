using Newtonsoft.Json;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    public class Ring
    {
        [JsonProperty("@type")]
        public string OType { get; set; }
        public Point[] Points { get; set; }
        public string Type { get; set; }
        public bool Passable { get; set; }
    }
}