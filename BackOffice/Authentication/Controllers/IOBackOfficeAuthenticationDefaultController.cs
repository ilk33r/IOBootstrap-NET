using System;
using IOBootstrap.NET.BackOffice.Authentication.ViewModels;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.Authentication.Controllers
{
    public class IOBackOfficeAuthenticationDefaultController : IOAuthenticationController<IOBackOfficeAuthenticationDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOBackOfficeAuthenticationDefaultController(IConfiguration configuration, IODatabaseContextDefaultImpl databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }
    }
}
