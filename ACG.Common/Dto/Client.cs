using System;

namespace ACG.Common.Dto
{
    public class Client
    {
        public Guid? Id { get; set; }
        public string ExternalId { get; set; }
        public string UserId { get; set; }
        public string ProducerCode { get; set; }
        public string Name { get; set; }
        public bool IsRegistered { get; set; }
    }
}
