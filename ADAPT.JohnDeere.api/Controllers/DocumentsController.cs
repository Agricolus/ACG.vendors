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
            var dataModels = await mediator.Send(new GetADAPTDocument() { UserId = userId, DocuemntId = documentId, DocumentFile = document });
            // var _jsonSerializer = new JsonSerializer
            // {
            //     NullValueHandling = NullValueHandling.Ignore,
            //     DefaultValueHandling = DefaultValueHandling.Ignore,
            //     TypeNameHandling = TypeNameHandling.None,
            //     ContractResolver = new AdaptContractResolver(),
            //     SerializationBinder = new InternalSerializationBinder(),
            //     ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //     Formatting = Formatting.None
            // };
            // using (var stream = new MemoryStream())
            // using (var reader = new StreamReader(stream))
            // using (var streamWriter = new StreamWriter(stream))
            // using (var textWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented })
            // {
            //     _jsonSerializer.Serialize(textWriter, dataModels);
            //     stream.Position = 0;
            //     // return reader.ReadToEnd();
            //     return Ok(reader);
            // }
            // return Ok(JsonConvert.DeserializeObject(dataModels));
            return Ok(dataModels);
        }

    }
}
