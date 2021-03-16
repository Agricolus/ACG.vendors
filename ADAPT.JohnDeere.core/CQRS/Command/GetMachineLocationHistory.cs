using System;
using System.Collections.Generic;
using ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse;
using MediatR;

namespace ADAPT.JohnDeere.core.CQRS.Command
{
    public class GetMachineLocationHistory : IRequest<List<ReportedLocation>>
    {
        public string UserId { get; set; }
        public Guid MachineId { get; set; }
    }
}
