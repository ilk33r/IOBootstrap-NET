using System;
using IOBootstrap.NET.BackOffice.Images.ViewModels;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.BackOffice.Images.Controllers
{
    public class IOBackOfficeImagesDefaultController : IOBackOfficeImagesController<IOBackOfficeImagesDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOBackOfficeImagesDefaultController(IConfiguration configuration, IODatabaseContextDefaultImpl databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }
    }
}
