using System;
using System.Collections.Generic;
using ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse;
using MediatR;

namespace ADAPT.JohnDeere.core.CQRS.Command
{
    public class ListOrganizationsFiles : IRequest<List<DocumentFile>>
    {
        public string UserId { get; set; }
    }
}
