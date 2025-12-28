using System;
using IOBootstrap.NET.BackOffice.PushNotification.ViewModels;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.BackOffice.PushNotification.Controllers;

[IOBackoffice]
[EnableCors]
[Produces("application/json")]
[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[IOSession]
public class IOPushNotificationBackOfficeDefaultController : IOPushNotificationBackOfficeController<IOPushNotificationBackOfficeDefaultViewModel, IODatabaseContextDefaultImpl, IOPushNotificationDevicesDefaultEntity>
{
    public IOPushNotificationBackOfficeDefaultController(IConfiguration configuration,
                                                         IWebHostEnvironment environment,
                                                         ILogger<IOLoggerType> logger,
                                                         IODatabaseContextDefaultImpl databaseContext) : base(configuration, environment, logger, databaseContext)
    {
    }
}
