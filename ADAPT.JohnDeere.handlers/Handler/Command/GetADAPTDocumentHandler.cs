using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using ACG.Common.CQRS.Event;
using ACG.Common.Service;

using ADAPT.JohnDeere.core.CQRS.Command;
using ADAPT.JohnDeere.core.CQRS.Query;
using ADAPT.JohnDeere.core.Dto;
using ADAPT.JohnDeere.core.Service;

using AgGateway.ADAPT.ADMPlugin.Json;
using AgGateway.ADAPT.ADMPlugin.Serializers;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using AgGateway.ADAPT.PluginManager;

using MediatR;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ADAPT.JohnDeere.handlers.Handler.Command
{
    public class GetADAPTDocumentHandler : IRequestHandler<GetADAPTDocument, ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse.DocumentFile>
    {
        private readonly PluginFactory pluginFactory;

        private readonly string appid;
        private readonly string iotAgentUrl;
        private readonly IJDApiClient jdApiClient;
        private readonly JohnDeereContext db;
        private readonly IMainApiClient mainApiClient;
        private readonly IMediator mediator;

        public GetADAPTDocumentHandler(IMediator mediator, IJDApiClient jdApiClient, JohnDeereContext db, IMainApiClient mainApiClient)
        {
            this.mediator = mediator;
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName().Name;
            string assemblyLocation = Path.GetDirectoryName(assembly.Location);
            var appPath = Path.Combine(assemblyLocation, assemblyName);
            this.pluginFactory = new PluginFactory(appPath);
            var modconfiguration = new ConfigurationBuilder()
                              .SetBasePath(assemblyLocation)
                              .AddJsonFile($"{assemblyName}.settings.json")
                              .Build();
            this.appid = modconfiguration.GetSection("ADAPTappId").Value;
            this.iotAgentUrl = modconfiguration.GetSection("iotAgentUrl").Value;
            this.jdApiClient = jdApiClient;
            this.db = db;
            this.mainApiClient = mainApiClient;
        }

        public async Task<ADAPT.JohnDeere.core.Dto.JohnDeereApiResponse.DocumentFile> Handle(GetADAPTDocument request, CancellationToken cancellationToken)
        {
            var doc = await (from d in db.DocumentFile where d.ExternalId == request.DocumentId select d).FirstOrDefaultAsync();
            if (doc == null) throw new FileNotFoundException("document is not in database");

            var accessToken = await mediator.Send(new GetUserToken() { UserId = request.UserId });
            var fileDownloadUrl = request.DocumentFile.Links.Where(l => l.Rel == "download").Select(l => l.Uri).First();
            var workDir = "adapt_documents\\johndeere";
            var downloadedFileName = Path.Combine(workDir, $"{request.DocumentId}.zip");
            await jdApiClient.Download(fileDownloadUrl, downloadedFileName, accessToken.AccessToken);

            List<string> pluginNames = pluginFactory.AvailablePlugins;
            var plugin = pluginFactory.GetPlugin(request.DocumentFile.Format);
            plugin.Initialize(appid);
            var importDirectory = Path.Combine(workDir, Path.GetFileNameWithoutExtension(downloadedFileName));
            var exportDirectory = $"{importDirectory}_iso";
            if (Directory.Exists(importDirectory))
                Directory.Delete(importDirectory, true);
            if (Directory.Exists(exportDirectory))
                Directory.Delete(exportDirectory, true);
            IEnumerable<ApplicationDataModel> dataModels = null;

            var admplugin = new AgGateway.ADAPT.ADMPlugin.Plugin();
            var pluginisoxml = new AgGateway.ADAPT.ISOv4Plugin.Plugin();
            List<DocumentFileOperation> resultObj = new List<DocumentFileOperation>();
            using (ZipArchive archive = ZipFile.OpenRead(downloadedFileName))
            {
                archive.ExtractToDirectory(importDirectory);
                if (plugin.IsDataCardSupported(importDirectory))
                {
                    var props = new Properties();
                    dataModels = plugin.Import(importDirectory, props);
                    for (int o = 0; o < dataModels.Count(); o++)
                    {
                        var dm = dataModels.ElementAt(o);
                        pluginisoxml.Export(dm, exportDirectory, props);
                        // admplugin.Export(dm, exportDirectory + "ADM", props);
                        var taskdataFile = File.OpenRead(Path.Combine(exportDirectory, "TASKDATA\\TASKDATA.XML"));
                        var sr = new StreamReader(taskdataFile);
                        var client = new WebClient();
                        client.Encoding = Encoding.UTF8;
                        client.Headers[HttpRequestHeader.ContentType] = "application/xml";
                        var iotaurl = new Uri(iotAgentUrl);
                        var xmlpayload = sr.ReadToEnd();
                        sr.Close();
                        client.UploadStringAsync(iotaurl, "POST", xmlpayload);
                        foreach (var loggedData in dm.Documents.LoggedData)
                        {
                            var grower = dm.Catalog.Growers.Where(g => g.Id.ReferenceId == loggedData.GrowerId).Select(g => g.Name).FirstOrDefault();
                            var farm = dm.Catalog.Farms.Where(f => f.Id.ReferenceId == loggedData.FarmId).Select(f => f.Description).FirstOrDefault();
                            var field = dm.Catalog.Fields.Where(f => f.Id.ReferenceId == loggedData.FieldId).Select(f => f.Description).FirstOrDefault();

                            var connectorsids = loggedData.EquipmentConfigurationGroup.EquipmentConfigurations.SelectMany(ec => new[] { ec.Connector1Id, ec.Connector2Id }).ToList();
                            var devicesconfigs = dm.Catalog.Connectors.Where(c => connectorsids.Contains(c.Id.ReferenceId)).Select(c => c.DeviceElementConfigurationId).ToList();
                            var devicesid = dm.Catalog.DeviceElementConfigurations.Where(dec => devicesconfigs.Contains(dec.Id.ReferenceId)).Select(dec => dec.DeviceElementId).ToList();
                            var currdevices = dm.Catalog.DeviceElements.Where(de => devicesid.Contains(de.Id.ReferenceId)).Select(de => new Equipment()
                            {
                                Description = de.Description,
                                Serial = de.SerialNumber,
                                Type = de.DeviceClassification.Value.Value.ToString()
                            }).ToList();
                            var operations = loggedData.OperationData.Select(od => od.OperationType.ToString()).ToList();
                            var loggedDataOut = new DocumentFileOperation()
                            {
                                Grower = grower,
                                Farm = farm,
                                Field = field,
                                Equipments = currdevices,
                                Operations = operations,
                                Points = new List<OperationPoint>()
                            };
                            resultObj.Add(loggedDataOut);
                            foreach (var operation in loggedData.OperationData)
                            {
                                var spatialRecords = operation.GetSpatialRecords != null ? operation.GetSpatialRecords() : null;
                                if (spatialRecords != null && spatialRecords.Any()) //No need to export a timelog if no data
                                {
                                    foreach (var spatialRecord in spatialRecords)
                                    {
                                        if (spatialRecord.Geometry != null && spatialRecord.Geometry as Point != null)
                                        {
                                            Point location = spatialRecord.Geometry as Point;
                                            var pointop = new OperationPoint()
                                            {
                                                X = location.X,
                                                Y = location.Y,
                                                Z = location.Z,
                                                Timestamp = spatialRecord.Timestamp
                                            };
                                            if (loggedDataOut.Points.Where(p => p.Timestamp == pointop.Timestamp).FirstOrDefault() == null)
                                                loggedDataOut.Points.Add(pointop);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (Directory.Exists(importDirectory))
                Directory.Delete(importDirectory, true);
            if (Directory.Exists(exportDirectory))
                Directory.Delete(exportDirectory, true);
            if (Directory.Exists(exportDirectory + "ADM"))
                Directory.Delete(exportDirectory + "ADM", true);
            if (File.Exists(downloadedFileName))
                File.Delete(downloadedFileName);


            foreach (var op in resultObj)
            {
                var machineSerial = op.Equipments.Where(e => e.Serial != null && e.Type == "Tractor").Select(om => om.Serial).FirstOrDefault();
                var machineId = db.Machines.Where(m => m.VIN == machineSerial).Select(m => m.Id).FirstOrDefault();
                var operation = String.Join("+", op.Operations);
                var points = op.Points.Select(p => new
                {
                    p.X,
                    p.Y,
                    p.Timestamp,
                    operation
                });
                await mainApiClient.Post<object>($"machines/{request.UserId.ToString()}/{machineId}/operationpoints", points);
            }
            
            doc.Processed = true;
            doc.ProcessedTime = DateTime.UtcNow;
            await db.SaveChangesAsync();

            var file = request.DocumentFile;
            file.Status = "PROCESSED";

            return file;
        }
 
   }
}
