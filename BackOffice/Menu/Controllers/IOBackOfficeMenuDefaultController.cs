using System;
using IOBootstrap.NET.BackOffice.Menu.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.Menu.Controllers
{
    [IOBackoffice]
    public class IOBackOfficeMenuDefaultController : IOBackOfficeMenuController<IOBackOfficeMenuDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOBackOfficeMenuDefaultController(IConfiguration configuration, IODatabaseContextDefaultImpl databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }
    }
}
