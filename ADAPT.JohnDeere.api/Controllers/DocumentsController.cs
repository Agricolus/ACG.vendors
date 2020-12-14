using System;
using System.Threading.Tasks;
using ADAPT.JohnDeere.core.CQRS.Command;
using ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse;
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

        [HttpGet("{userId}")]
        public async Task<IActionResult> ListDocuments(string userId, string documentId)
        {
            var organizationsfiles = await mediator.Send(new ListOrganizationsFiles() { UserId = userId });
            return Ok(organizationsfiles);
        }

        [HttpPost("{userId}/{documentId}")]
        public async Task<IActionResult> ImportDocument(string userId, string documentId, [FromBody] DocumentFile document)
        {
            var whatever = await mediator.Send(new GetADAPTDocument() { UserId = userId, DocuemntId = documentId, DocumentFile = document });
            return Ok(whatever);
        }

    }
}
