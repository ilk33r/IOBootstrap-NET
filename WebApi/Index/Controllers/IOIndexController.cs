using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Controllers;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.WebApi.Index.ViewModels;

namespace IOBootstrap.NET.WebApi.Index.Controllers
{
    public class IOIndexController<TViewModel, TDBContext> : IOController<TViewModel, TDBContext>
    where TDBContext : IODatabaseContext<TDBContext> 
    where TViewModel : IOIndexViewModel<TDBContext>, new()
    {
        public IOIndexController(IConfiguration configuration, 
                                 IWebHostEnvironment environment,
                                 ILogger<IOLoggerType> logger,
                                 TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
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
