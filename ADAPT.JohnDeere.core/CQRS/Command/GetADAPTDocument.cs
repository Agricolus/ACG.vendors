using ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse;
using MediatR;

namespace ADAPT.JohnDeere.core.CQRS.Command
{
    public class GetADAPTDocument : IRequest<DocumentFile>
    {
        public string UserId { get; set; }
        public string DocumentId { get; set; }
        public DocumentFile DocumentFile { get; set; }
    }
}
