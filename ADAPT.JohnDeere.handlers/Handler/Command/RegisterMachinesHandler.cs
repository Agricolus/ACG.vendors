using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ADAPT.JohnDeere.core.CQRS.Command;
using ADAPT.JohnDeere.core.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ACG.Common.Dto;
using ACG.Common.Service;
using ADAPT.JohnDeere.core.Service;

namespace ADAPT.JohnDeere.handlers.Handler.Command
{
    public class RegisterMachinesHandler : IRequestHandler<RegisterMachines, List<MachineRegistration>>,
                                            IRequestHandler<RegisterMachine, Machine>
    {
        private readonly JohnDeereContext db;
        private readonly IMainApiClient mainApiClient;
        private readonly IMapper mapper;
        private readonly IJDApiClient jdApiClient;

        public RegisterMachinesHandler(JohnDeereContext db, IMainApiClient mainApiClient, IMapper mapper, IJDApiClient jdApiClient)
        {
            this.db = db;
            this.mainApiClient = mainApiClient;
            this.mapper = mapper;
            this.jdApiClient = jdApiClient;
        }
        public async Task<List<MachineRegistration>> Handle(RegisterMachines request, CancellationToken cancellationToken)
        {
            var registered = await (from ms in db.Machines where ms.UserId == request.UserId select ms).ToListAsync();
            var newregistrations = request.Machines.Where(m => !registered.Select(r => r.ExternalId).Contains(m.ExternalId)).ToList();
            if (newregistrations.Count() != 0)
            {
                var registrations = mapper.Map<List<Model.Machine>>(newregistrations);
                db.Machines.AddRange(registrations);
                await db.SaveChangesAsync();
                var all = registered.Concat(registrations);
                return mapper.Map<List<MachineRegistration>>(all);
            }
            return mapper.Map<List<MachineRegistration>>(registered);
        }

        public async Task<Machine> Handle(RegisterMachine request, CancellationToken cancellationToken)
        {
            var machine = request.Machine;
            var registeredMachine = await (from ms in db.Machines where ms.Id == machine.Id select ms).FirstOrDefaultAsync();
            if (registeredMachine == null) return null;

            var response = await mainApiClient.Post<Machine>("machines/import/producer", machine);
            response.IsRegistered = true;

            registeredMachine.RegistrationTime = DateTime.UtcNow;
            registeredMachine.SyncTime = machine.PTime;

            await db.SaveChangesAsync();


            return response;
        }
    }
}
