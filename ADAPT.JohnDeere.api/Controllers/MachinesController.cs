using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADAPT.JohnDeere.core.CQRS.Command;
using ADAPT.JohnDeere.core.CQRS.Query;
using ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse;
using ADAPT.JohnDeere.core.Service;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace ADAPT.JohnDeere.api.Controllers
{
    [ApiController]
    [Route("johndeere/[controller]")]
    public class MachinesController : ControllerBase
    {
        private IMediator mediator;
        private readonly IConfiguration configuration;
        private readonly IJDApiClient apiclient;

        public MachinesController(IMediator mediator, IConfiguration configuration, IJDApiClient apiclient)
        {
            this.mediator = mediator;
            this.configuration = configuration;
            this.apiclient = apiclient;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> ListMachinesFromSource(string userId)
        {
            var machines = await mediator.Send(new RetrieveMachines() { UserId = userId });
            return Ok(machines);
        }

        [HttpPost("{userId}/{machineId}")]
        public async Task<IActionResult> ListMachinesFromSource(string userId, ACG.Common.Dto.Machine machine)
        {
            var machines = await mediator.Send(new RegisterMachine() { Machine = machine });
            return Ok(machines);
        }

        

    }
} 
