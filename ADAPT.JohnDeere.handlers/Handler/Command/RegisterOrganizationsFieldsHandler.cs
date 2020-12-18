using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ACG.Common.Dto;
using ACG.Common.Service;
using ADAPT.JohnDeere.core.CQRS.Command;
using ADAPT.JohnDeere.core.Dto;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ADAPT.JohnDeere.handlers.Handler.Command
{
    public class RegisterOrganizationsFieldsHandler : IRequestHandler<RegisterOrganizationField, Field>, IRequestHandler<RegisterOrganizationsFields, List<FieldRegistration>>
                                                , IRequestHandler<RegisterOrganizationsClients, List<ClientRegistration>>
    {
        private readonly JohnDeereContext db;
        private readonly IMainApiClient mainApiClient;
        private readonly IMapper mapper;

        public RegisterOrganizationsFieldsHandler(JohnDeereContext db, IMainApiClient mainApiClient, IMapper mapper)
        {
            this.db = db;
            this.mainApiClient = mainApiClient;
            this.mapper = mapper;
        }

        public async Task<Field> Handle(RegisterOrganizationField request, CancellationToken cancellationToken)
        {
            var fieldArea = request.Field.Boundaries.ToList().Sum(b => b.Area.ValueAsDouble);
            // var clientId = request.Field.Clients.Clients.Select(c => c.Id).FirstOrDefault();
            var owningOrganizationId = request.Field.Links.Where(l => l.Rel == "owningOrganization").Select(l => l.Uri.Split("/").Last()).FirstOrDefault();
            double[][][][] boundaries = request.Field.Boundaries.Select(b => b.Multipolygons.Select(m => m.Rings.Select(r => r.Points.Select(p => new double[] { p.Lat, p.Lon }).ToArray()).ToArray()).ToArray()).FirstOrDefault();
            Field field = new Field()
            {
                Area = fieldArea,
                ExternalId = request.Field.Id.ToString(),
                ClientId = "our_client_id_from_clients_import_not_external_client_id",
                Name = request.Field.Name,
                ProducerCode = "johndeere",
                Boundaries = boundaries
            };
            var clientName = request.Field.Clients.Clients.First().Name;
            Client clent = new Client()
            {
                Name = clientName
            };
            return field;
        }

        public async Task<List<FieldRegistration>> Handle(RegisterOrganizationsFields request, CancellationToken cancellationToken)
        {
            var registered = await (from f in db.Fields where f.UserId == request.UserId select f).ToListAsync();
            var newregistrations = request.Fields.Where(f => !registered.Select(r => r.ExternalId).Contains(f.ExternalId)).ToList();
            if (newregistrations.Count() != 0)
            {
                var registrations = mapper.Map<List<Model.Field>>(newregistrations);
                db.Fields.AddRange(registrations);
                await db.SaveChangesAsync();
                registered = registered.Concat(registrations).ToList();
            }
            return mapper.Map<List<FieldRegistration>>(registered);
        }

        public async Task<List<ClientRegistration>> Handle(RegisterOrganizationsClients request, CancellationToken cancellationToken)
        {
            var registered = await (from f in db.Clients where f.UserId == request.UserId select f).ToListAsync();
            var newregistrations = request.Clients.Where(f => !registered.Select(r => r.ExternalId).Contains(f.ExternalId)).ToList();
            if (newregistrations.Count() != 0)
            {
                var registrations = mapper.Map<List<Model.Client>>(newregistrations);
                db.Clients.AddRange(registrations);
                await db.SaveChangesAsync();
                registered = registered.Concat(registrations).ToList();
            }
            return mapper.Map<List<ClientRegistration>>(registered);
        }
    }
}
