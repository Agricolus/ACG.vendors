using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ADAPT.JohnDeere.core.CQRS.Command;
using ADAPT.JohnDeere.core.CQRS.Query;
using ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse;
using ADAPT.JohnDeere.core.Service;
using ADAPT.JohnDeere.handlers.Service;
using MediatR;

namespace ADAPT.JohnDeere.handlers.Handler.Command
{
    public class ListOrganizationsFilesHandler : IRequestHandler<ListOrganizationsFiles, List<DocumentFile>>
    {
        private readonly IMediator mediator;
        private readonly IJDApiClient jdapiclient;

        public ListOrganizationsFilesHandler(IMediator mediator, IJDApiClient jdclient)
        {
            this.mediator = mediator;
            this.jdapiclient = jdclient;
            
        }
        public async Task<List<DocumentFile>> Handle(ListOrganizationsFiles request, CancellationToken cancellationToken)
        {
            var accessToken = await mediator.Send(new GetUserToken() { UserId = request.UserId });
            var organizations = await jdapiclient.Get<Response<Organization>>("/organizations", accessToken.AccessToken);

            // var machines = new List<ACG.Common.Dto.Machine>();
            var documentsFiles = new List<DocumentFile>();

             foreach (var org in organizations.Values)
            {
                var orgfileslink = org.Links.Where(l => l.Rel == "files").Select(l => l.Uri).FirstOrDefault();
                if (orgfileslink == null) continue;
                var orgfiles = await jdapiclient.Get<Response<DocumentFile>>($"{orgfileslink};count=100", accessToken.AccessToken);
                foreach (var file in orgfiles.Values)
                {
                    documentsFiles.Add(file);
                }
            }

            return documentsFiles;
        }
    }
}
