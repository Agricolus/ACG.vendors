using System;

namespace ADAPT.JohnDeere.core.Dto
{
    public class FieldRegistration
    {
        public Guid? Id { get; set; }
        public string UserId { get; set; }
        public string ExternalId { get; set; }
        public int OwningOrganizationId { get; set; }
        public string ClientId { get; set; }
        public DateTime? RegistrationTime { get; set; }
    }
}
