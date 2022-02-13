using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.Index.ViewModels;

namespace IOBootstrap.NET.MW.WebApi.Index.Controllers
{
    public class IOMWIndexDefaultController : IOMWIndexController<IOMWIndexDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOMWIndexDefaultController(IConfiguration configuration, 
                                          IODatabaseContextDefaultImpl databaseContext, 
                                          IWebHostEnvironment environment, 
                                          ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }
    }
}
