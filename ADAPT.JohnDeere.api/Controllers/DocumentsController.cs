using System.IO;
using System.Threading.Tasks;
using ADAPT.JohnDeere.core.CQRS.Command;
using ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse;
using AgGateway.ADAPT.ADMPlugin.Json;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            var adaptDocument = await mediator.Send(new GetADAPTDocument() { UserId = userId, DocumentId = documentId, DocumentFile = document });
            return Ok(adaptDocument);
        }

    }
}
