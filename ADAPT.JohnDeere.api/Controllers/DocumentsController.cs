using System;
using System.Threading.Tasks;
using ADAPT.JohnDeere.core.CQRS.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ADAPT.JohnDeere.api.Controllers
{
    [ApiController]
    [Route("johndeere/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IMediator mediator;

        public DocumentsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("{userId}/{documentId}")]
        public async Task<IActionResult> ListDocuments(string userId, string documentId)
        {
            var whatever = await mediator.Send(new GetADAPTDocument() { UserId = userId, DocuemntId = documentId });
            return Ok(whatever);
        }

    }
}
