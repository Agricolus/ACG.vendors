using System;
using System.Collections.Generic;
using ACG.Common.Dto;
using ADAPT.JohnDeere.core.Dto;
using MediatR;

namespace ADAPT.JohnDeere.core.CQRS.Command
{
    public class ListOrganizationsFields : IRequest<FieldsClients>
    {
        public string UserId { get; set; }
    }
}
