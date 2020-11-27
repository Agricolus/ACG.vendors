using System;

namespace ACG.Common.Dto
{
    public class Machine
    {
        public Guid? Id { get; set; }
        public string ExternalId { get; set; }
        public string UserId { get; set; }
        public string ProducerCode { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string ProducerCommercialName { get; set; }
        public DateTime? PTime { get; set; }
        public object OtherData { get; set; }
        public bool IsRegistered { get; set; }
    }
}
