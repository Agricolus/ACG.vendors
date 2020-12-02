using System;
using System.Collections.Generic;
using MediatR;

namespace ADAPT.JohnDeere.core.CQRS.Command
{
    public class GetADAPTDocument : IRequest<List<string>>
    {
        public string UserId { get; set; }
        public string DocuemntId { get; set; }
    }
}
