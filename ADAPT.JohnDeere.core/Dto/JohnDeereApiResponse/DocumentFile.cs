using System;
using Newtonsoft.Json;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    public class DocumentFile
    {
        [JsonProperty("@type")]
        public string OType { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime CreatedTime { get; set; }

        public DateTime ModifiedTime { get; set; }

        public int NativeSize { get; set; }
        public string Source { get; set; }

        public bool TransferPending { get; set; }

        public string VisibleViaShare { get; set; }
        public bool Shared { get; set; }
        public bool New { get; set; }

        public string Status { get; set; }
        public bool Archived { get; set; }
        public string Format { get; set; }
        public string Manufacturer { get; set; }
        public bool DelayProcessing { get; set; }

        public Link[] Links { get; set; }
    }
}
