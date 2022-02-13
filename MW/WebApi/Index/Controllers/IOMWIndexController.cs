using System;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.MW.Core.Controllers;
using IOBootstrap.NET.MW.DataAccess.Context;
using IOBootstrap.NET.MW.WebApi.Index.ViewModels;

namespace IOBootstrap.NET.MW.WebApi.Index.Controllers
{
    public class IOMWIndexController<TViewModel, TDBContext> : IOMWController<TViewModel, TDBContext> where TViewModel : IOMWIndexViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Initialization Methods

        public IOMWIndexController(IConfiguration configuration, 
                                   TDBContext databaseContext, 
                                   IWebHostEnvironment environment, 
                                   ILogger<IOLoggerType> logger) : base(configuration, databaseContext, environment, logger)
        {
        }

        #endregion

        #region Default

        public virtual IOResponseModel Index()
        {
            // Obtain app version
            string appVersion = Configuration.GetValue<string>(IOConfigurationConstants.Version);

            // Create response status model
            IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.OK,
                                                                             "IO Bootstrapt MW.",
                                                                             true,
                                                                             String.Format("Version: {0}", appVersion));

            // Return response
            return new IOResponseModel(responseStatus);
        }

        #endregion

        #region Helper Methods

        public override bool EncryptResult()
        {
            return false;
        }

        #endregion
    }
}
