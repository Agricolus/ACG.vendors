using System;
using Newtonsoft.Json;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    public class Field
    {
        [JsonProperty("@type")]
        public string OType { get; set; }
        public string Name { get; set; }
        public ClientsList Clients { get; set; }
        public Boundary[] Boundaries { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public bool Archived { get; set; }
        public Guid Id { get; set; }
        public Link[] Links { get; set; }
    }
}
