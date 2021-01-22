using System;
using System.Collections.Generic;
using ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse;
using MediatR;
using AgGateway.ADAPT.ApplicationDataModel.ADM;

namespace ADAPT.JohnDeere.core.CQRS.Command
{
    public class GetADAPTDocument : IRequest<string>
    {
        public string UserId { get; set; }
        public string DocuemntId { get; set; }
        public DocumentFile DocumentFile { get; set; }
    }
}
