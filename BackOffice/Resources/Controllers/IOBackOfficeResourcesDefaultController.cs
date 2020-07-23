using System;
using IOBootstrap.NET.BackOffice.Resources.ViewModels;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.Resources.Controllers
{
    public class IOBackOfficeResourcesDefaultController : IOBackOfficeResourcesController<IOBackOfficeResourcesDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOBackOfficeResourcesDefaultController(IConfiguration configuration, IODatabaseContextDefaultImpl databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }
    }
}
