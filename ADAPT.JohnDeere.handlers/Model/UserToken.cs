using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAPT.JohnDeere.handlers.Model
{
    [Table("users", Schema = "johndeere")]
    public class UserToken
    {
        [StringLength(64)]
        public String Id { get; set; }
        [StringLength(64)]
        public String ExternalId { get; set; }
        [StringLength(1024)]
        public string AccessToken { get; set; }
        [StringLength(64)]
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public DateTime RegistrationTime { get; set; }
    }
}
