using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ACG.Common.CQRS.Event;
using ADAPT.JohnDeere.core.CQRS.Command;
using AgGateway.ADAPT.PluginManager;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace ADAPT.JohnDeere.handlers.Handler.Command
{
    public class GetADAPTDocumentHandler : IRequestHandler<GetADAPTDocument, List<string>>
    {
        private readonly PluginFactory pluginFactory;

        private readonly string appid;
        private readonly IMediator mediator;

        public GetADAPTDocumentHandler(IMediator mediator)
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
        }

        public async Task<List<string>> Handle(GetADAPTDocument request, CancellationToken cancellationToken)
        {
            List<string> pluginNames = pluginFactory.AvailablePlugins;
            // foreach (string pluginName in pluginNames)
            // {
            //     Console.WriteLine($"{pluginName}");
            //     IPlugin plugin = _factory.GetPlugin(pluginName);
            //     plugin.Initialize(appid);
            //     for (int i = 0; i < datafilesPaths.Count(); i++)
            //     {
            //         var currentAdaptDataPath = Path.Combine(adaptDataTmpPath, Path.GetFileNameWithoutExtension(datafilesPaths[i]));
            //         var currentAdaptDataPathOut = Path.Combine(adaptDataOutPath, Path.GetFileNameWithoutExtension(datafilesPaths[i]));


            //         Console.WriteLine($"\t testing {Path.GetFileName(datafilesPaths[i])}");
            //         using (ZipArchive archive = ZipFile.OpenRead(datafilesPaths[i]))
            //         {
            //             if (!Directory.Exists(currentAdaptDataPath))
            //                 archive.ExtractToDirectory(currentAdaptDataPath);
            //             if (plugin.IsDataCardSupported(currentAdaptDataPath))
            //             {
            //                 Console.WriteLine($"{pluginName} support {Path.GetFileName(datafilesPaths[i])}");
            //                 IEnumerable<ApplicationDataModel> dataModels = plugin.Import(currentAdaptDataPath);
            //                 for (int o = 0; o < dataModels.Count(); o++)
            //                 {
            //                     var dm = dataModels.ElementAt(o);
            //                     pluginisoxml.Export(dm, currentAdaptDataPathOut, new Properties());
            //                     ZipFile.CreateFromDirectory(currentAdaptDataPathOut, $"{currentAdaptDataPathOut}.zip");
            //                     Directory.Delete(currentAdaptDataPathOut, true);
            //                 }
            //             }
            //         }
            //     }
            //     Console.WriteLine();
            // }
            // await mediator.Publish(new ConvertADAPTtoISOXML() {} );
            return pluginNames;
        }
    }
}
