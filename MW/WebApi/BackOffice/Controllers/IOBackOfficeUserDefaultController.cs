using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.BackOffice.ViewModels;

namespace IOBootstrap.NET.MW.WebApi.BackOffice.Controllers
{
    public class IOBackOfficeUserDefaultController : IOUserController<IOBackOfficeUserDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOBackOfficeUserDefaultController(IConfiguration configuration, 
                                                 IODatabaseContextDefaultImpl databaseContext, 
                                                 IWebHostEnvironment environment, 
                                                 ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }
    }
}