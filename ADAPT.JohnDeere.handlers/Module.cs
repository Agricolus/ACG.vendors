using System;
using ACG.Common;
using System.Composition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MediatR;
using ADAPT.JohnDeere.core.Service;
using ADAPT.JohnDeere.handlers.Service;
using AutoMapper;
using ADAPT.JohnDeere.handlers.Model;
using ADAPT.JohnDeere.core.Dto;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace ADAPT.JohnDeere.handlers
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
            services.AddScoped<IJDApiClient, JDApiClient>();
        }
    }

    public class JohnDeereMappingsProfile : Profile
    {
        public JohnDeereMappingsProfile()
        {
            CreateMap<Machine, MachineRegistration>();
            CreateMap<MachineRegistration, Machine>().ForAllMembers(opt => opt.Condition((source, destination, sourceMember, destMember) => (sourceMember != null)));
        }
    }
}
