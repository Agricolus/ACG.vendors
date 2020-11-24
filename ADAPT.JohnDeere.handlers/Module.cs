using System;
using ACG.Common;
using System.Composition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MediatR;
using ADAPT.JohnDeere.handlers;

namespace ADAPT.JohnDeere.api
{
    public class Module : IModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            switch (configuration["DataBaseType"])
            {
                case "postgre": services.AddDbContext<JohnDeereContext>(options => options.UseNpgsql(configuration.GetConnectionString("ACGPostgreContext"))); break;
                // case "mysql": services.AddDbContext<StationsContext>(options => options.UseMySql(configuration.GetConnectionString("ACGMySqlContext"))); break;
            }

            services.AddMediatR(Assembly.GetExecutingAssembly());
        }

    }
}
