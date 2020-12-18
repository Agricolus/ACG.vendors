using System;

namespace ADAPT.JohnDeere.core.Dto
{
    public class ClientRegistration
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string ExternalId { get; set; }
        public DateTime? RegistrationTime { get; set; }
    }
}
