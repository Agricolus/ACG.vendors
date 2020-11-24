using System;
using ADAPT.JohnDeere.core.Dto;
using MediatR;

namespace ADAPT.JohnDeere.core.CQRS.Command
{
    public class RefreshUserAccessToken : IRequest<UserToken>
    {
        public string RefreshToken { get; set; }
    }
}
