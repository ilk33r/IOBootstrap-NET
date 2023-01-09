using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.WebApi.PushNotification.ViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.WebApi.PushNotification.Controllers
{
    [EnableCors]
    [Produces("application/json")]
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class IOPushNotificationDefaultController : IOPushNotificationController<IOPushNotificationDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOPushNotificationDefaultController(IConfiguration configuration, 
                                                   IWebHostEnvironment environment, 
                                                   ILogger<IOLoggerType> logger,
                                                   IODatabaseContextDefaultImpl databaseContext) : base(configuration, environment, logger, databaseContext)
        {
        }
    }
}
