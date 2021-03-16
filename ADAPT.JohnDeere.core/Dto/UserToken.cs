using System;

namespace ADAPT.JohnDeere.core.Dto
{
    public class UserToken
    {
        public string UserId { get; set; }
        public string ExternalUserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public DateTime RegistrationTime { get; set; }
    }
}
