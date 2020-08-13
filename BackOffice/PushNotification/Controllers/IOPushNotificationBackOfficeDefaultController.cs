using System;
using IOBootstrap.NET.BackOffice.PushNotification.ViewModels;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.PushNotification.Controllers
{
    public class IOPushNotificationBackOfficeDefaultController : IOPushNotificationBackOfficeController<IOPushNotificationBackOfficeDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOPushNotificationBackOfficeDefaultController(IConfiguration configuration, IODatabaseContextDefaultImpl databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }
    }
}
