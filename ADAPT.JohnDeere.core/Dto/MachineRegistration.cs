using System;

namespace ADAPT.JohnDeere.core.Dto
{
    public class MachineRegistration
    {
        public Guid? Id { get; set; }
        public string UserId { get; set; }
        public String ExternalId { get; set; }
        public int OrganizationId { get; set; }
        public DateTime? SyncTime { get; set; }
        public DateTime? RegistrationTime { get; set; }
        public string VIN { get; set; }
    }
}
