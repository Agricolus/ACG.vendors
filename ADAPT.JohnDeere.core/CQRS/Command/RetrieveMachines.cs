using System;
using System.Collections.Generic;
using ACG.Common.Dto;
using MediatR;

namespace ADAPT.JohnDeere.core.CQRS.Command
{
    public class RetrieveMachines : IRequest<List<Machine>>
    {
        public string UserId { get; set; }
    }
}
