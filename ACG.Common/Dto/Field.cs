using System;

namespace ACG.Common.Dto
{
    public class Field
    {
        public Guid? Id { get; set; }
        public string ExternalId { get; set; }
        public string UserId { get; set; }
        public string ProducerCode { get; set; }
        public string Name { get; set; }
        public double Area { get; set; }
        public string ClientId { get; set; }
        public double[][][][] Boundaries { get; set; }
        public double[][][][] UnpassableBoundaries { get; set; }
        public bool IsRegistered { get; set; }
    }
}
