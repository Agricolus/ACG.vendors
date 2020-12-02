using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ADAPT.JohnDeere.core.CQRS.Command;
using AgGateway.ADAPT.PluginManager;
using MediatR;

namespace ADAPT.JohnDeere.handlers.Handler.Command
{
    public class GetADAPTDocumentHandler : IRequestHandler<GetADAPTDocument, List<string>>
    {
        private readonly PluginFactory pluginFactory;

        public GetADAPTDocumentHandler()
        {
            string temp = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            string temp2 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var appPath = "";
            var me = Assembly.GetExecutingAssembly();
            Console.WriteLine($"ME I AM {me.GetName()}");
            this.pluginFactory = new PluginFactory(Path.Combine(appPath, @"Plugins"));
        }
        
        public async Task<List<string>> Handle(GetADAPTDocument request, CancellationToken cancellationToken)
        {
            List<string> pluginNames = pluginFactory.AvailablePlugins;

            return pluginNames;
        }
    }
}
