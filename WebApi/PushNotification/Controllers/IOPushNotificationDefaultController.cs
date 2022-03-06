using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.WebApi.PushNotification.ViewModels;

namespace IOBootstrap.NET.WebApi.PushNotification.Controllers
{
    public class IOPushNotificationDefaultController : IOPushNotificationController<IOPushNotificationDefaultViewModel>
    {
        public IOPushNotificationDefaultController(IConfiguration configuration, 
                                                   IWebHostEnvironment environment, 
                                                   ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }
    }
}
