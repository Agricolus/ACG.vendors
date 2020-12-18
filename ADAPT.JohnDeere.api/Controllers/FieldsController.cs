using System;
using System.Linq;
using System.Threading.Tasks;
using ADAPT.JohnDeere.core.CQRS.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ADAPT.JohnDeere.api.Controllers
{

    [ApiController]
    [Route("johndeere/[controller]")]
    public class FieldsController : ControllerBase
    {
        private readonly IMediator mediator;

        public FieldsController(IMediator mediator)
        {
            this.mediator = mediator;

        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> ListFieldsAndClients(string userId)
        {
            var fields = await mediator.Send(new ListOrganizationsFields() { UserId = userId });
            return Ok(fields);
        }


        [HttpPost("{userId}")]
        public async Task<IActionResult> RegisterFieldsAndClient([FromBody] core.Dto.JohnDeereApiResponse.Field field, string userId)
        {
            var res = await mediator.Send(new RegisterOrganizationField() { Field = field });
            return Ok(res);
        }


        [HttpPost("{userId}/bulk")]
        public async Task<IActionResult> RegisterBulkFieldsAndClient([FromBody] core.Dto.JohnDeereApiResponse.Field[] fields, string userId)
        {
            // var res = await mediator.Send(new RegisterOrganizationsFields() { UserId = userId, Fields = fields.ToList() });
            // return Ok(res);
            return Ok();
        }
    }
}
