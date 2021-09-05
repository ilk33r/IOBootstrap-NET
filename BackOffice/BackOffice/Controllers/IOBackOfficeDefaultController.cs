using System;
using IOBootstrap.NET.BackOffice.BackOffice.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.BackOffice.Controllers
{
    [IOBackoffice]
    public class IOBackOfficeDefaultController : IOBackOfficeController<IOBackOfficeDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOBackOfficeDefaultController(IConfiguration configuration, IODatabaseContextDefaultImpl databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }
    }
}
