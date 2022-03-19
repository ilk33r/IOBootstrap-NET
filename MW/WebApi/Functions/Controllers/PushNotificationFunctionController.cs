using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.MW.Core.Controllers;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.Functions.ViewModels;

namespace IOBootstrap.NET.MW.WebApi.Functions.Controllers
{
    public class PushNotificationFunctionController<TViewModel, TDBContext> : IOMWController<TViewModel, TDBContext> where TViewModel : PushNotificationFunctionViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        public PushNotificationFunctionController(IConfiguration configuration, TDBContext databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }
    }
}
