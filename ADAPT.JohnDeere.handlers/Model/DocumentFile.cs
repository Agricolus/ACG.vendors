using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAPT.JohnDeere.handlers.Model
{
    [Table("documentfile", Schema = "johndeere")]
    public class DocumentFile
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        [StringLength(64)]
        public String ExternalId { get; set; }

        public int OrganizationId { get; set; }

        public bool Processed { get; set; }
    }
}
