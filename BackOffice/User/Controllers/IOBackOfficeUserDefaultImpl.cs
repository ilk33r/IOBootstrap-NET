using System;
using IOBootstrap.NET.BackOffice.User.ViewModels;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.WebApi.User.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.User.Controllers
{
    public class IOBackOfficeUserDefaultImpl : IOUserController<IOBackOfficeUserDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOBackOfficeUserDefaultImpl(IConfiguration configuration, IODatabaseContextDefaultImpl databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }
    }
}
