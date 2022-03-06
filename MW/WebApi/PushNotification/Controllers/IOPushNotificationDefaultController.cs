using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.PushNotification.ViewModels;

namespace IOBootstrap.NET.MW.WebApi.PushNotification.Controllers
{
    public class IOPushNotificationDefaultController : IOPushNotificationController<IOPushNotificationDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOPushNotificationDefaultController(IConfiguration configuration, 
                                                   IODatabaseContextDefaultImpl databaseContext, 
                                                   IWebHostEnvironment environment, 
                                                   ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }
    }
}
