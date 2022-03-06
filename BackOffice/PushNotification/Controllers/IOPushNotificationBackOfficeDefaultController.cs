using System;
using IOBootstrap.NET.BackOffice.PushNotification.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;

namespace IOBootstrap.NET.BackOffice.PushNotification.Controllers
{
    [IOBackoffice]
    public class IOPushNotificationBackOfficeDefaultController : IOPushNotificationBackOfficeController<IOPushNotificationBackOfficeDefaultViewModel>
    {
        public IOPushNotificationBackOfficeDefaultController(IConfiguration configuration, 
                                                             IWebHostEnvironment environment, 
                                                             ILogger<IOLoggerType> logger) : base(configuration, environment, logger)
        {
        }
    }
}
