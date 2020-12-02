using System;
using ACG.Common;
using System.Composition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MediatR;

namespace ADAPT.JohnDeere.api
{
    public class Module : IModule
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

    }
}
