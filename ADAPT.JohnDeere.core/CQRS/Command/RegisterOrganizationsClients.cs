using System;
using System.Collections.Generic;
using ADAPT.JohnDeere.core.Dto;
using MediatR;

namespace ADAPT.JohnDeere.core.CQRS.Command
{
    public class RegisterOrganizationsClients: IRequest<List<ClientRegistration>>
    {
        public string UserId { get; set; }
        public List<ClientRegistration> Clients { get; set; }
    }
}
