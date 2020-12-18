using System;
using System.Threading.Tasks;
using ADAPT.JohnDeere.core.CQRS.Command;
using ADAPT.JohnDeere.core.Service;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ADAPT.JohnDeere.api.Controllers
{
    [ApiController]
    [Route("johndeere/[controller]")]
    public class MachinesController : ControllerBase
    {
        private IMediator mediator;
        private readonly IConfiguration configuration;
        private readonly IJDApiClient jdApiClient;

        public MachinesController(IMediator mediator, IConfiguration configuration, IJDApiClient jdApiClient)
        {
            this.mediator = mediator;
            this.configuration = configuration;
            this.jdApiClient = jdApiClient;
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
            machine.OtherData = JsonConvert.DeserializeObject<object>(machine.OtherData.ToString());
            var machines = await mediator.Send(new RegisterMachine() { Machine = machine });
            return Ok(machines);
        }


        [HttpGet("{userId}/{machineId}")]
        public async Task<IActionResult> MachineLocations(string userId, Guid machineId)
        {
            var locationHistory = await mediator.Send(new GetMachineLocationHistory() { UserId = userId, MachineId = machineId });
            return Ok(locationHistory);
        }



    }
}
