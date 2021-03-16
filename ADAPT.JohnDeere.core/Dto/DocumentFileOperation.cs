using System;
using System.Collections.Generic;

namespace ADAPT.JohnDeere.core.Dto
{
    public class DocumentFileOperation
    {
        public string Grower { get; set; }
        public string Farm { get; set; }
        public string Field { get; set; }
        public List<Equipment> Equipments { get; set; }
        public List<string> Operations { get; set; }
        public List<OperationPoint> Points { get; set; }
    }

    public class Equipment
    {
        public string Description { get; set; }
        public string Serial { get; set; }
        public string Type { get; set; }
    }

    public class OperationPoint {
        public double X { get; set; }
        public double Y { get; set; }
        public double? Z { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
