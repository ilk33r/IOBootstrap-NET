using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.WebApi.Index.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.WebApi.Index.Controllers
{
    public class IOIndexController : IOController<IOIndexViewModel, IODatabaseContextDefaultImpl>
    {
        public IOIndexController(IConfiguration configuration, 
                                IODatabaseContextDefaultImpl databaseContext,
                                IWebHostEnvironment environment,
                                ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        #region Default

        // [Obsolete("This Method is Deprecated", false)]
        public virtual IOResponseModel Index()
        {
            // Obtain app version
            string appVersion = Configuration.GetValue<string>(IOConfigurationConstants.Version);

            // Create response status model
            IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.OK,
                                                                             "IO Bootstrapt.",
                                                                             true,
                                                                             String.Format("Version: {0}", appVersion));

            // Return response
            return new IOResponseModel(responseStatus);
        }

        #endregion
    }
}
