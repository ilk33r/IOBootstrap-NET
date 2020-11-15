using System;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.WebApi.Handshake.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.WebApi.Handshake.Controllers
{
    public class IOHandshakeDefaultController : IOHandshakeController<IOHandshakeDefaultViewModel, IODatabaseContextDefaultImpl>
    {
        public IOHandshakeDefaultController(IConfiguration configuration, IODatabaseContextDefaultImpl databaseContext, IWebHostEnvironment environment, ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }
    }
}
