using System;
using MediatR;

namespace ADAPT.JohnDeere.core.CQRS.Command
{
    public class CreateOrUpdateUserRegistration : IRequest<bool>
    {
        public string UserId { get; set; }
        public string ExternalUserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
    }
}
