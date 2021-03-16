using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ADAPT.JohnDeere.core.CQRS.Command;
using ADAPT.JohnDeere.core.CQRS.Query;
using ADAPT.JohnDeere.core.Dto;
using ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse;
using ADAPT.JohnDeere.core.Service;
using MediatR;

namespace ADAPT.JohnDeere.handlers.Handler.Command
{
    public class ListOrganizationsFieldsHandler : IRequestHandler<ListOrganizationsFields, FieldsClients>
    {
        private readonly JohnDeereContext db;
        private readonly IMediator mediator;
        private readonly IJDApiClient jdApiClient;

        public ListOrganizationsFieldsHandler(JohnDeereContext db, IMediator mediator, IJDApiClient jdApiClient)
        {
            this.db = db;
            this.mediator = mediator;
            this.jdApiClient = jdApiClient;
        }
        public async Task<FieldsClients> Handle(ListOrganizationsFields request, CancellationToken cancellationToken)
        {
            var accessToken = await mediator.Send(new GetUserToken() { UserId = request.UserId });
            var organizations = await jdApiClient.Get<Response<Organization>>("/organizations", accessToken.AccessToken);
            var fields = new List<Field>();
            foreach (var org in organizations.Values)
            {
                var orgFieldsLink = org.Links.Where(l => l.Rel == "fields").Select(l => l.Uri).FirstOrDefault();
                if (orgFieldsLink == null) continue;
                var start = 0;
                var count = 100;
                var orgFields = await jdApiClient.Get<Response<Field>>($"{orgFieldsLink};start={start};count={count}?status=AVAILABLE&embed=activeBoundary,clients", accessToken.AccessToken);
                fields.AddRange(orgFields.Values);
                while (Array.Exists(orgFields.Links, l => l.Rel == "nextPage"))
                {
                    start += count;
                    orgFields = await jdApiClient.Get<Response<Field>>($"{orgFieldsLink};start={start};count={count}?status=AVAILABLE&embed=activeBoundary,clients", accessToken.AccessToken);
                    fields.AddRange(orgFields.Values);
                }
            }
            var outfields = new List<ACG.Common.Dto.Field>();
            var fieldsregs = new List<FieldRegistration>();
            var outclients = new List<ACG.Common.Dto.Client>();
            var clientsregs = new List<ClientRegistration>();
            foreach (var c in fields.Where(f => f.Boundaries != null && f.Boundaries.Length > 0).SelectMany(f => f.Clients.Clients).GroupBy(c => c.Id).Select(g => g.First()))
            {
                var clientId = c.Id.ToString();
                var clientName = c.Name;
                ACG.Common.Dto.Client client = new ACG.Common.Dto.Client()
                {
                    ProducerCode = "johndeere",
                    ExternalId = clientId,
                    UserId = request.UserId,
                    Name = clientName
                };
                outclients.Add(client);
                clientsregs.Add(new ClientRegistration()
                {
                    ExternalId = client.ExternalId,
                    UserId = request.UserId
                });
            }
            var clientsRegistrations = await mediator.Send(new RegisterOrganizationsClients()
            {
                UserId = request.UserId,
                Clients = clientsregs,
            });
            foreach (var of in outclients)
            {
                var fr = clientsRegistrations.Find(f => f.ExternalId == of.ExternalId);
                of.Id = fr.Id;
                of.IsRegistered = fr.RegistrationTime != null;
            }

            foreach (var f in fields.Where(f => f.Boundaries != null && f.Boundaries.Length > 0 && f.Archived == false && f.Boundaries.Any(b => b.Active)))
            {
                var clientId = f.Clients.Clients.First().Id.ToString();
                var extclientId = clientsRegistrations.Find(c => c.ExternalId == clientId).Id.ToString();
                var fieldArea = f.Boundaries.ToList().Sum(b => b.Area.ValueAsDouble);
                var owningOrganizationId = f.Links.Where(l => l.Rel == "owningOrganization").Select(l => l.Uri.Split("/").Last()).FirstOrDefault();
                double[][][] boundaries = f.Boundaries.SelectMany(b => b.Multipolygons.Select(m => m.Rings.Where(r => r.Passable).Select(r => r.Points.Select(p => new double[] { p.Lat, p.Lon }).ToArray()).ToArray()).ToArray()).FirstOrDefault();
                double[][][] unpassableBoundaries = f.Boundaries.SelectMany(b => b.Multipolygons.Select(m => m.Rings.Where(r => !r.Passable).Select(r => r.Points.Select(p => new double[] { p.Lat, p.Lon }).ToArray()).ToArray()).ToArray()).FirstOrDefault();
                ACG.Common.Dto.Field field = new ACG.Common.Dto.Field()
                {
                    UserId = request.UserId,
                    Area = fieldArea,
                    ExternalId = f.Id.ToString(),
                    ClientId = extclientId,
                    Name = f.Name,
                    ProducerCode = "johndeere",
                    Boundaries = boundaries,
                    UnpassableBoundaries = unpassableBoundaries,
                    ModificationTime = f.LastModifiedTime
                };
                outfields.Add(field);
                fieldsregs.Add(new FieldRegistration()
                {
                    ExternalId = field.ExternalId,
                    OwningOrganizationId = Int32.Parse(owningOrganizationId),
                    UserId = request.UserId,
                    ClientId = extclientId
                });
            }
            var fieldsRegistrations = await mediator.Send(new RegisterOrganizationsFields()
            {
                UserId = request.UserId,
                Fields = fieldsregs,
            });
            foreach (var of in outfields)
            {
                var fr = fieldsRegistrations.Find(f => f.ExternalId == of.ExternalId);
                of.Id = fr.Id;
                of.IsRegistered = fr.RegistrationTime != null;
            }
            return new FieldsClients()
            {
                Fields = outfields.ToArray(),
                Clients = outclients.ToArray()
            };
        }
    }
}
