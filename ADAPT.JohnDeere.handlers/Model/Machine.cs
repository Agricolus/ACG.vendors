using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAPT.JohnDeere.handlers.Model
{
    [Table("machines", Schema = "johndeere")]
    public class Machine
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        [StringLength(64)]
        public String ExternalId { get; set; }

        public int OrganizationId { get; set; }

        public DateTime? SyncTime { get; set; }

        public DateTime? RegistrationTime { get; set; }
    }
}
