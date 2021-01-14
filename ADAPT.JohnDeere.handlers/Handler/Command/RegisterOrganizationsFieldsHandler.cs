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
            var field = request.Field;
            var registeredField = await (from ms in db.Fields where ms.Id == field.Id select ms).FirstOrDefaultAsync();
            if (registeredField == null) return null;

            var response = await mainApiClient.Post<Field>("fields/import/producer", field);
            response.IsRegistered = true;

            registeredField.RegistrationTime = field.ModificationTime;

            await db.SaveChangesAsync();

            return response;
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
