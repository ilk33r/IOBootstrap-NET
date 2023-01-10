using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.FunctionsApi.PushNotificationFunction.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace IOBootstrap.NET.FunctionsApi.PushNotificationFunction.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PushNotificationFunctionDefaultController : PushNotificationFunctionController<PushNotificationFunctionDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public PushNotificationFunctionDefaultController(IConfiguration configuration, IWebHostEnvironment environment, ILogger<IOLoggerType> logger, IODatabaseContextDefaultImpl databaseContext) : base(configuration, environment, logger, databaseContext)
        {
        }
    }
}
