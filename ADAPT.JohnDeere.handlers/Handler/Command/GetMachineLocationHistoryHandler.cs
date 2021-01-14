using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ACG.Common.Service;
using ADAPT.JohnDeere.core.CQRS.Command;
using ADAPT.JohnDeere.core.CQRS.Query;
using ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse;
using ADAPT.JohnDeere.core.Service;
using FIWARE;
using FIWARE.ContextBroker.Enums;
using FIWARE.ContextBroker.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ADAPT.JohnDeere.handlers.Handler.Command
{
    public class GetMachineLocationHistoryHandler : IRequestHandler<GetMachineLocationHistory, List<ReportedLocation>>
    {
        private readonly JohnDeereContext db;
        private readonly IJDApiClient jdApiClient;
        private readonly IMediator mediator;
        private readonly IMainApiClient mainApiClient;
        private readonly IConfiguration configuration;

        public GetMachineLocationHistoryHandler(JohnDeereContext db, IMediator mediator, IJDApiClient jdApiClient, IMainApiClient mainApiClient, IConfiguration configuration)
        {
            this.db = db;
            this.jdApiClient = jdApiClient;
            this.mediator = mediator;
            this.mainApiClient = mainApiClient;
            this.configuration = configuration;
        }
        
        public async Task<List<ReportedLocation>> Handle(GetMachineLocationHistory request, CancellationToken cancellationToken)
        {
            var accessToken = await mediator.Send(new GetUserToken() { UserId = request.UserId });
            var machine = await (from m in db.Machines where m.Id == request.MachineId select m).FirstOrDefaultAsync();
            if (machine == null) throw new Exception("machine not registered");
            var count = 100;
            var start = 0;
            var startDate = machine.SyncTime.Value;
            var currentTime = DateTime.UtcNow;
            var endDate = currentTime;
            var locations = new List<ReportedLocation>();
            do
            {
                endDate = new DateTime(startDate.Year, startDate.Month, 1, 0, 0, 0).AddMonths(1).AddSeconds(-1);
                if (endDate > currentTime) endDate = currentTime;
                var machineLocationHistory = await jdApiClient.Get<Response<ReportedLocation>>($"/machines/{machine.ExternalId}/locationHistory;start={start};count={count}?startDate={startDate.ToString("o", System.Globalization.CultureInfo.InvariantCulture)}&endDate={endDate.ToString("o", System.Globalization.CultureInfo.InvariantCulture)}", accessToken.AccessToken);
                locations.AddRange(machineLocationHistory.Values);
                while (Array.Exists(machineLocationHistory.Links, l => l.Rel == "nextPage"))
                {
                    start += count;
                    machineLocationHistory = await jdApiClient.Get<Response<ReportedLocation>>($"/machines/{machine.ExternalId}/locationHistory;start={start};count={count}?startDate={startDate.ToString("o", System.Globalization.CultureInfo.InvariantCulture)}&endDate={endDate.ToString("o", System.Globalization.CultureInfo.InvariantCulture)}", accessToken.AccessToken);
                    locations.AddRange(machineLocationHistory.Values);
                }
                start = 0;
                startDate = endDate.AddSeconds(1);
            }
            while (endDate.Month < currentTime.Month);

            var cbConfig = configuration.GetSection("contextBroker");
            var cbUrl = cbConfig.GetValue<string>("cbUrl");
            var cbService = cbConfig.GetValue<string>("cbService");
            var cbServicePath = cbConfig.GetValue<string>("cbServicePath");
            var contextBrokerNotificationEndpoint = cbConfig.GetValue<string>("machineNoditiciationCB");

            var cbClient = new ContextBrokerClient(cbService, cbServicePath, cbUrl);
            foreach (var location in locations)
            {
                await cbClient.UpdateEntity<object>(machine.Id.ToString(), new {
                    Position = new {
                        type = "Point",
                        coordinates = new double[]{ location.Point.Lat, location.Point.Lon}
                    },
                    PTime = location.EventTimestamp
                }, "tractor", AttributesFormatEnum.keyValues);
                // await mainApiClient.Post<object>("machines/notification", new
                // {
                //     Data = new[]
                //     {
                //         new {
                //             Id = machine.Id,
                //             Position = new
                //             {
                //                 Type = "Point",
                //                 Coordinates = new double[] { location.Point.Lat, location.Point.Lon }
                //             },
                //             PTime = location.EventTimestamp
                //         }
                //     }
                // });
            }
            
            machine.SyncTime = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return locations;
        }
    }
}
