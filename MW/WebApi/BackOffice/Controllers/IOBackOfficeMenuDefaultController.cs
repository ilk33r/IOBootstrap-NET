using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.Controllers
{
    public class IOBackOfficeMenuDefaultController : IOBackOfficeMenuController<IOBackOfficeMenuDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOBackOfficeMenuDefaultController(IConfiguration configuration, 
                                                 IODatabaseContextDefaultImpl databaseContext, 
                                                 IWebHostEnvironment environment, 
                                                 ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }
    }
}
