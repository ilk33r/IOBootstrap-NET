using System;
using IOBootstrap.NET.BackOffice.Configuration.ViewModels;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.Configuration.Controllers
{
    public class IOBackOfficeConfigurationDefaultController : IOBackOfficeConfigurationsController<IOBackOfficeConfigurationControllerDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOBackOfficeConfigurationDefaultController(IConfiguration configuration, IODatabaseContextDefaultImpl databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }
    }
}
