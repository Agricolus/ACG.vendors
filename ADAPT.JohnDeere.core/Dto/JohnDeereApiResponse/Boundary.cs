using System;
using Newtonsoft.Json;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    public class Boundary
    {
        [JsonProperty("@type")]
        public string OType { get; set; }
        public string Name { get; set; }
        public string SourceType { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime ModifiedTime { get; set; }
        public Area Area { get; set; }
        public Polygon[] Multipolygons { get; set; }
        public Extent Extent { get; set; }
        public bool Archived { get; set; }
        public Guid Id { get; set; }
        public Link[] Links { get; set; }
        public bool Active { get; set; }
        public bool Irrigated { get; set; }
    }
}
