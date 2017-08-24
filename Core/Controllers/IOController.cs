using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Common.Models.SystemUpTime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace IOBootstrap.NET.Core.Controllers
{
    public abstract class IOController<TLogger> : Controller
    {

        #region Properties

        private ILogger<TLogger> Logger;
        private ILoggerFactory LoggerFactory;

        #endregion

        public IOController(ILoggerFactory factory, ILogger<TLogger> logger)
        {
            // Setup properties
            this.Logger = logger;
            this.LoggerFactory = factory;

            logger.LogDebug("Request start: {0}", this.GetType().Name);
        }

        #region Default

        public virtual IOResponseModel Index()
        {
            // Create response status model
            IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.OK,
                                                                         "IO Bootstrapt.",
                                                                         true,
                                                                         String.Format("Version: {0}", IOCommonHelpers.CurrentVersion));

            // Return response
            return new IOResponseModel(responseStatus);
        }

        #endregion

        #region Errors

        public virtual IOResponseModel Error404()
        {
            // Update response status code
            this.Response.StatusCode = 404;

            // Create response status model
            IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.ENDPOINT_FAILURE,
                                                                         String.Format("Path {0} is invalid.", this.Request.Path));

            // Return response
            return new IOResponseModel(responseStatus);
        }

        #endregion

    }
}
