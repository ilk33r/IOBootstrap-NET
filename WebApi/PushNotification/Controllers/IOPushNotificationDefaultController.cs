using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.WebApi.PushNotification.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
