using System;
using MAD.JsonConverters.NestedJsonConverterNS;
using Newtonsoft.Json;

namespace ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse
{
    [JsonConverter(typeof(NestedJsonConverter))]
    public class Machine
    {
        [JsonProperty("@type")]
        public string OType { get; set; }

        public string VisualizationCategory { get; set; }

        [JsonProperty("category.name")]
        public string Category { get; set; }

        [JsonProperty("make.name")]
        public string Make { get; set; }

        [JsonProperty("model.name")]
        public string Model { get; set; }

        [JsonProperty("detailMachineCode.name")]
        public string DetailMachineCode { get; set; }

        public string ProductKey { get; set; }

        public string EngineSerialNumber { get; set; }

        public string TelematicsState { get; set; }

        [JsonProperty("GUID")]
        public Guid Guid { get; set; }

        public int ModelYear { get; set; }

        public int Id { get; set; }

        public string Vin { get; set; }

        public string Name { get; set; }

        [JsonProperty("equipmentMake.name")]
        public string EquipmentMake { get; set; }

        [JsonProperty("equipmentType.name")]
        public string EquipmentType { get; set; }

        [JsonProperty("equipmentApexType.name")]
        public string EquipmentApexType { get; set; }

        [JsonProperty("equipmentModel.name")]
        public string EquipmentModel { get; set; }

        public Link[] Links { get; set; }

        [JsonProperty("currentLocation.point.lat")]
        public double Lat { get; set; }

        [JsonProperty("currentLocation.point.lon")]
        public double Lng { get; set; }

        [JsonProperty("currentLocation.eventTimestamp")]
        public DateTime PositionTime { get; set; }
    }
}
