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
using AgGateway.ADAPT.ADMPlugin.Serializers;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.PluginManager;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

using AgGateway.ADAPT.ADMPlugin.Json;
using static GeoJSONPlugin.PluginProperties;

namespace ADAPT.JohnDeere.handlers.Handler.Command
{
    public class GetADAPTDocumentHandler : IRequestHandler<GetADAPTDocument, string>
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

        public async Task<string> Handle(GetADAPTDocument request, CancellationToken cancellationToken)
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
            IEnumerable<ApplicationDataModel> dataModels = null;


            // var fuffolo = JsonConvert.SerializeObject(dataModels);
            var admplugin = new AgGateway.ADAPT.ADMPlugin.Plugin();
            var admserializer = new AgGateway.ADAPT.ADMPlugin.Serializers.AdmSerializer();
            var geojsonplugin = new GeoJSONPlugin.Plugin();
            // var fuffolo = admplugin.Export;
            string jsonresponse = null;
            using (ZipArchive archive = ZipFile.OpenRead(downloadedFileName))
            {
                archive.ExtractToDirectory(importDirectory);
                if (plugin.IsDataCardSupported(importDirectory))
                {
                    var props = new Properties();
                    props.SetProperty("Anonymise", false.ToString());
                    props.SetProperty("ApplyingAnonymiseValuesPer", ApplyingAnonymiseValuesEnum.PerAdm.ToString());
                    dataModels = plugin.Import(importDirectory, props);
                    var pluginisoxml = new AgGateway.ADAPT.ISOv4Plugin.Plugin();
                    for (int o = 0; o < dataModels.Count(); o++)
                    {
                        var dm = dataModels.ElementAt(o);
                        pluginisoxml.Export(dm, exportDirectory, props);
                        admplugin.Export(dm, exportDirectory + "ADM", props);
                        var dm2 = admplugin.Import(exportDirectory + "ADM");
                        // props.SetProperty("MaximumMappingDepth", 0.ToString());
                        geojsonplugin.Export(dm, exportDirectory + "GEOJSON", props);
                        jsonresponse = await AgGateway.ADAPT.ADMPlugin.Json.MyBaseJsonSerializer.Instance.Serialize(dm2[0]);
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
            // if (Directory.Exists(exportDirectory + "ADM"))
            //     Directory.Delete(exportDirectory + "ADM", true);
            if (File.Exists(downloadedFileName))
                File.Delete(downloadedFileName);



            return jsonresponse;
        }
    }
}




namespace AgGateway.ADAPT.ADMPlugin.Json
{
    public class MyBaseJsonSerializer : IBaseSerializer
    {
        private static MyBaseJsonSerializer _instance;

        public static MyBaseJsonSerializer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MyBaseJsonSerializer();
                }
                return _instance;
            }
        }

        private readonly JsonSerializer _jsonSerializer;

        private MyBaseJsonSerializer()
        {
            _jsonSerializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None,
                ContractResolver = new AdaptContractResolver(),
                SerializationBinder = new InternalSerializationBinder(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            };
        }

        public async Task<string> Serialize(AgGateway.ADAPT.ApplicationDataModel.ADM.ApplicationDataModel dataModel)
        {
            try
            {
                using (var stream = new MemoryStream())
                using (var reader = new StreamReader(stream))
                using (var streamWriter = new StreamWriter(stream))
                using (var textWriter = new JsonTextWriter(streamWriter))
                {
                    _jsonSerializer.Serialize(textWriter, dataModel);
                    stream.Position = 0;
                    return await reader.ReadToEndAsync();
                }
            }
            finally
            {
            }
        }

        public T Deserialize<T>()
        {
            throw new NotImplementedException();
        }

        public void SerializeWithLengthPrefix<T>(IEnumerable<T> content, string path) where T : new()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> DeserializeWithLengthPrefix<T>(string path) where T : new()
        {
            throw new NotImplementedException();
        }

        public void Serialize<T>(T content, string path)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(string path)
        {
            throw new NotImplementedException();
        }
    }
}