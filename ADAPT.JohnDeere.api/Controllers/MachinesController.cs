using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
            var accessToken = await mediator.Send(new GetUserToken() { UserId = userId });
            var organizations = await apiclient.Call<Response<Organization>>("/organizations", accessToken.AccessToken);

            var machines = new List<object>();
            foreach (var org in organizations.Values)
            {
                var orgmachinelink = org.Links.Where(l => l.Rel == "machines").Select(l => l.Uri).FirstOrDefault();
                if (orgmachinelink == null) continue;
                var orgmachine = await apiclient.Call<Response<Machine>>(orgmachinelink, accessToken.AccessToken);
                foreach (var machine in orgmachine.Values)
                {
                    var r = new
                    {
                        orgname = org.Name,
                        machine.Category,
                        machine.DetailMachineCode,
                        machine.EngineSerialNumber,
                        machine.EquipmentApexType,
                        machine.EquipmentMake,
                        machine.EquipmentModel,
                        machine.EquipmentType,
                        machine.Guid,
                        machine.Id,
                        machine.Make,
                        machine.Model,
                        machine.ModelYear,
                        machine.Name,
                        machine.ProductKey,
                        machine.TelematicsState,
                        machine.Vin,
                        machine.VisualizationCategory
                    };
                    machines.Add(r);
                }
            }
            return Ok(machines);
        }
    }
}
