using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAPT.JohnDeere.handlers.Model
{
    [Table("clients", Schema = "johndeere")]
    public class Client
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        [StringLength(64)]
        public string ExternalId { get; set; }

        public DateTime? RegistrationTime { get; set; }
    }
}
