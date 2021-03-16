using System;
using ADAPT.JohnDeere.core.Dto;
using MediatR;

namespace ADAPT.JohnDeere.core.CQRS.Query
{
    public class GetUserToken : IRequest<UserToken>
    {
        public string UserId { get; set; }
    }
}
