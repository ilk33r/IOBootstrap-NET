using System;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.WebApi.Index.ViewModels;

namespace IOBootstrap.NET.WebApi.Index.Controllers
{
    public class IOIndexDefaultController : IOIndexController<IOIndexDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOIndexDefaultController(IConfiguration configuration, 
                                        IWebHostEnvironment environment,
                                        ILogger<IOLoggerType> logger,
                                        IODatabaseContextDefaultImpl databaseContext) : base(configuration, environment, logger, databaseContext)
        {
        }
    }
}
