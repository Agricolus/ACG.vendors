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
    public class RetrieveMachinesHandler : IRequestHandler<RetrieveMachines, List<ACG.Common.Dto.Machine>>
    {
        private readonly JohnDeereContext db;
        private readonly IJDApiClient jdapi;
        private readonly IMediator mediator;

        public RetrieveMachinesHandler(JohnDeereContext db, IJDApiClient jdapi, IMediator mediator)
        {
            this.db = db;
            this.jdapi = jdapi;
            this.mediator = mediator;
        }

        public async Task<List<ACG.Common.Dto.Machine>> Handle(RetrieveMachines request, CancellationToken cancellationToken)
        {
            var accessToken = await mediator.Send(new GetUserToken() { UserId = request.UserId });
            var organizations = await jdapi.Get<Response<Organization>>("/organizations", accessToken.AccessToken);

            var machines = new List<ACG.Common.Dto.Machine>();
            var machinesRegistrations = new List<MachineRegistration>();
            foreach (var org in organizations.Values)
            {
                var orgmachinelink = org.Links.Where(l => l.Rel == "machines").Select(l => l.Uri).FirstOrDefault();
                if (orgmachinelink == null) continue;
                var orgmachine = await jdapi.Get<Response<core.Dto.JohnDeereApiResponse.Machine>>($"{orgmachinelink}?embed=breadcrumbs", accessToken.AccessToken);
                foreach (var machine in orgmachine.Values)
                {
                    var r = new ACG.Common.Dto.Machine()
                    {
                        ExternalId = machine.Id.ToString(),
                        UserId = request.UserId,
                        ProducerCode = "johndeere",
                        Lat = machine.Lat,
                        Lng = machine.Lng,
                        Name = machine.Name,
                        Model = machine.Model,
                        Code = machine.DetailMachineCode,
                        Type = machine.Category.ToLower(),
                        Description = machine.EquipmentType,
                        ProducerCommercialName = machine.EquipmentMake,
                        PTime = machine.PositionTime,
                        OtherData = new
                        {
                            OrganizationName = org.Name,
                            machine.EngineSerialNumber,
                            machine.EquipmentApexType,
                            machine.EquipmentModel,
                            machine.Guid,
                            machine.Make,
                            machine.ModelYear,
                            machine.ProductKey,
                            machine.TelematicsState,
                            machine.Vin,
                            machine.VisualizationCategory,
                        }
                    };
                    machines.Add(r);
                    machinesRegistrations.Add(new MachineRegistration()
                    {
                        ExternalId = machine.Id.ToString(),
                        OrganizationId = org.Id,
                        UserId = request.UserId
                    });
                }
            }
            machinesRegistrations = await mediator.Send(new RegisterMachines()
            {
                UserId = request.UserId,
                Machines = machinesRegistrations
            });
            foreach (var registerdMachine in machinesRegistrations)
            {
                var machine = machines.Find(m => m.ExternalId == registerdMachine.ExternalId);
                machine.Id = registerdMachine.Id;
                machine.IsRegistered = registerdMachine.RegistrationTime != null;
            }
            return machines;
        }
    }
}
