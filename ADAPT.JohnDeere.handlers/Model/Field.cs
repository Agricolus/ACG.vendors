using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAPT.JohnDeere.handlers.Model
{
    [Table("fields", Schema = "johndeere")]
    public class Field
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        [StringLength(64)]
        public string ExternalId { get; set; }

        public int OwningOrganizationId { get; set; }

        [StringLength(64)]
        public string ClientId { get; set; }

        public DateTime? RegistrationTime { get; set; }
    }
}
