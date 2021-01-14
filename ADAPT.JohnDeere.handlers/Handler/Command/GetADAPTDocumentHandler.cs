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

namespace ADAPT.JohnDeere.handlers.Handler.Command
{
    public class GetADAPTDocumentHandler : IRequestHandler<GetADAPTDocument, IEnumerable<ApplicationDataModel>>
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

        public async Task<IEnumerable<ApplicationDataModel>> Handle(GetADAPTDocument request, CancellationToken cancellationToken)
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
            var admplugin = new AgGateway.ADAPT.ADMPlugin.Plugin(AgGateway.ADAPT.ADMPlugin.SerializationVersionEnum.V1);
            var admserializer = new AgGateway.ADAPT.ADMPlugin.Serializers.AdmSerializer(AgGateway.ADAPT.ADMPlugin.SerializationVersionEnum.V1);
            // var fuffolo = admplugin.Export;

            using (ZipArchive archive = ZipFile.OpenRead(downloadedFileName))
            {
                archive.ExtractToDirectory(importDirectory);
                if (plugin.IsDataCardSupported(importDirectory))
                {
                    dataModels = plugin.Import(importDirectory);
                    var pluginisoxml = new AgGateway.ADAPT.ISOv4Plugin.Plugin();


                    for (int o = 0; o < dataModels.Count(); o++)
                    {
                        var dm = dataModels.ElementAt(o);
                        pluginisoxml.Export(dm, exportDirectory, new Properties());
                        AgGateway.ADAPT.ADMPlugin.Json.MyBaseJsonSerializer.Instance.Serialize<ApplicationDataModel>(dm, exportDirectory + "JSON");
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



            return dataModels;
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
        TypeNameHandling = TypeNameHandling.All,
        ContractResolver = new AdaptContractResolver(),
        SerializationBinder = new InternalSerializationBinder()
      };
    }

    public void Serialize<T>(T dataModel, string path)
    {
      var tempPath = Path.GetTempFileName();
      try
      {
        using (var fileStream = File.Open(tempPath, FileMode.Create, FileAccess.ReadWrite))
        using (var streamWriter = new StreamWriter(fileStream))
        using (var textWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.Indented })
        {
          _jsonSerializer.Serialize(textWriter, dataModel);
        }
        ZipUtil.Zip(path, tempPath);
      }
      finally
      {
        try
        {
          File.Delete(tempPath);
        }
        catch { }
      }
    }

    public T Deserialize<T>(string path)
    {
      var tempPath = Path.GetTempFileName();
      try
      {
        ZipUtil.Unzip(path, tempPath);

        using (var fileStream = File.Open(tempPath, FileMode.Open))
        using (var streamReader = new StreamReader(fileStream))
        using (var textReader = new InternalJsonTextReader(streamReader))
        {
          return _jsonSerializer.Deserialize<T>(textReader);
        }
      }
      finally
      {
        try
        {
          File.Delete(tempPath);
        }
        catch { }
      }
    }

    public void SerializeWithLengthPrefix<T>(IEnumerable<T> content, string path) where T : new()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<T> DeserializeWithLengthPrefix<T>(string path) where T : new()
    {
      throw new NotImplementedException();
    }
  }
}