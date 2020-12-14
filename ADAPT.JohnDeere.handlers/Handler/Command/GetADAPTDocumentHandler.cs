using System;
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
using ACG.Common.CQRS.Event;
using ADAPT.JohnDeere.core.CQRS.Command;
using ADAPT.JohnDeere.core.CQRS.Query;
using ADAPT.JohnDeere.core.Service;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.PluginManager;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace ADAPT.JohnDeere.handlers.Handler.Command
{
    public class GetADAPTDocumentHandler : IRequestHandler<GetADAPTDocument, List<string>>
    {
        private readonly PluginFactory pluginFactory;

        private readonly string appid;
        private readonly string iotAgentUrl;
        private readonly IJDApiClient jdApiClient;
        private readonly IMediator mediator;

        public GetADAPTDocumentHandler(IMediator mediator, IJDApiClient jdApiClient)
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
        }

        public async Task<List<string>> Handle(GetADAPTDocument request, CancellationToken cancellationToken)
        {
            var accessToken = await mediator.Send(new GetUserToken() { UserId = request.UserId });
            var fileDownloadUrl = request.DocumentFile.Links.Where(l => l.Rel == "download").Select(l => l.Uri).First();
            var workDir = "adapt_documents\\johndeere";
            var downloadedFileName = Path.Combine(workDir, $"{request.DocuemntId}.zip");
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
            using (ZipArchive archive = ZipFile.OpenRead(downloadedFileName))
            {
                archive.ExtractToDirectory(importDirectory);
                if (plugin.IsDataCardSupported(importDirectory))
                {
                    IEnumerable<ApplicationDataModel> dataModels = plugin.Import(importDirectory);
                    var pluginisoxml = new AgGateway.ADAPT.ISOv4Plugin.Plugin();

                    for (int o = 0; o < dataModels.Count(); o++)
                    {
                        var dm = dataModels.ElementAt(o);
                        pluginisoxml.Export(dm, exportDirectory, new Properties());
                        var taskdataFile = File.OpenRead(Path.Combine(exportDirectory, "TASKDATA\\TASKDATA.XML"));
                        var sr = new StreamReader(taskdataFile);
                        var client = new WebClient();
                        client.Encoding = Encoding.UTF8;
                        client.Headers[HttpRequestHeader.ContentType] = "application/xml";
                        var iotaurl = new Uri(iotAgentUrl);
                        var xmlpayload = sr.ReadToEnd();
                        sr.Close();
                        client.UploadStringAsync(iotaurl, "POST", xmlpayload);
                    }
                }
            }
            if (Directory.Exists(importDirectory))
                Directory.Delete(importDirectory, true);
            if (Directory.Exists(exportDirectory))
                Directory.Delete(exportDirectory, true);
            if (File.Exists(downloadedFileName))
                File.Delete(downloadedFileName);

            return pluginNames;
        }
    }
}
