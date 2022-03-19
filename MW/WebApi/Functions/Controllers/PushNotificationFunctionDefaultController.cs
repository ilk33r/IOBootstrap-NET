using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.Functions.ViewModels;

namespace IOBootstrap.NET.MW.WebApi.Functions.Controllers
{
    public class PushNotificationFunctionDefaultController : PushNotificationFunctionController<PushNotificationFunctionDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public PushNotificationFunctionDefaultController(IConfiguration configuration, IODatabaseContextDefaultImpl databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }
    }
}
