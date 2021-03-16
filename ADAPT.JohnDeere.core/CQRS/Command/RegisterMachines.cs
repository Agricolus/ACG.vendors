using System;
using System.Collections.Generic;
using ADAPT.JohnDeere.core.Dto;
using MediatR;

namespace ADAPT.JohnDeere.core.CQRS.Command
{
    public class RegisterMachines : IRequest<List<MachineRegistration>>
    {

        public string UserId { get; set; }
        public List<MachineRegistration> Machines { get; set; }
    }
}
