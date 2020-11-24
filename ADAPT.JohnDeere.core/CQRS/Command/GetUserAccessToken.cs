using System;
using ADAPT.JohnDeere.core.Dto;
using MediatR;

namespace ADAPT.JohnDeere.core.CQRS.Command
{
    public class GetUserAccessToken : IRequest<AccessTokenResponse>
    {
         public string Code { get; set; }
    }
}
