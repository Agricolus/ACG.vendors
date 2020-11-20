using System;
using ACG.Common;
using System.Composition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace ADAPT.JohnDeere.api
{
    public class Module : IModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            //     .AddJsonFile("modulesettings.json", optional: true, reloadOnChange: true)
            //     .AddJsonFile($"modulesettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);
            // var moduleConfiguration = (IModuleConfiguration)builder.Build();
            // services.AddSingleton<IModuleConfiguration>(moduleConfiguration);
        }

    }

    // public interface IModuleConfiguration : IConfiguration { }
}
