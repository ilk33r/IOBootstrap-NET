using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.WebApi.Index.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace IOBootstrap.NET.WebApi.Index.Controllers
{
    public class IOIndexController : IOController<IOIndexViewModel, IODatabaseContextDefaultImpl>
    {
        public IOIndexController(IConfiguration configuration, IODatabaseContextDefaultImpl databaseContext, IWebHostEnvironment environment) : base(configuration, databaseContext, environment)
        {
        }

        #region Default

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
