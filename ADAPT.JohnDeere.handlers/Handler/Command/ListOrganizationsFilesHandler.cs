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
using Microsoft.EntityFrameworkCore;

namespace ADAPT.JohnDeere.handlers.Handler.Command
{
    public class ListOrganizationsFilesHandler : IRequestHandler<ListOrganizationsFiles, List<DocumentFile>>
    {
        private readonly IMediator mediator;
        private readonly IJDApiClient jdApiClient;
        private readonly JohnDeereContext db;

        public ListOrganizationsFilesHandler(IMediator mediator, IJDApiClient jdApiClient, JohnDeereContext db)
        {
            this.mediator = mediator;
            this.jdApiClient = jdApiClient;
            this.db = db;
        }

        public async Task<List<DocumentFile>> Handle(ListOrganizationsFiles request, CancellationToken cancellationToken)
        {
            var accessToken = await mediator.Send(new GetUserToken() { UserId = request.UserId });
            var organizations = await jdApiClient.Get<Response<Organization>>("/organizations", accessToken.AccessToken);

            // var machines = new List<ACG.Common.Dto.Machine>();
            var documentsFiles = new List<DocumentFile>();

            foreach (var org in organizations.Values)
            {
                var orgfileslink = org.Links.Where(l => l.Rel == "files").Select(l => l.Uri).FirstOrDefault();
                if (orgfileslink == null) continue;
                var orgid = org.Id;
                var orgfiles = await jdApiClient.Get<Response<DocumentFile>>($"{orgfileslink};count=100?fileType=4&fileType=1", accessToken.AccessToken);
                var fileids = orgfiles.Values.Select(f => f.Id).ToList();
                var registeredocs = await (from d in db.DocumentFile where d.OrganizationId == orgid && fileids.Contains(d.ExternalId) select d).ToListAsync();
                foreach (var file in orgfiles.Values)
                {
                    documentsFiles.Add(file);
                    var doc = registeredocs.Find(d => d.ExternalId == file.Id);
                    if (doc == null)
                    {
                        var docfile = new Model.DocumentFile()
                        {
                            OrganizationId = orgid,
                            ExternalId = file.Id,
                            Processed = false,
                            SourceMachineSerial = file.Source,
                            UserId = request.UserId,
                            DownloadUrl = file.Links.Where(l => l.Rel == "download").Select(l => l.Uri).FirstOrDefault(),
                            ModifiedTime = file.ModifiedTime,
                        };
                        db.Add(docfile);
                    }
                    else
                    {
                        file.Status = doc.Processed ? "PROCESSED" : file.Status;
                        if (file.ModifiedTime > doc.ModifiedTime)
                        {
                            file.Status =  doc.Processed ? "MODIFIED AFTER PROCESS" : file.Status;
                            doc.ModifiedTime = file.ModifiedTime;
                        }
                    }
                }
            }
            await db.SaveChangesAsync();

            return documentsFiles;
        }
    }
}
