using System;
using System.Collections.Generic;
using ACG.Common.Dto;
using ADAPT.JohnDeere.core.Dto;
using MediatR;

namespace ADAPT.JohnDeere.core.CQRS.Command
{
    public class RegisterOrganizationsFields : IRequest<List<FieldRegistration>>
    {
        public string UserId { get; set; }
        public List<FieldRegistration> Fields { get; set; }
    }
}
