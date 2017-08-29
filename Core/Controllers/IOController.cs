using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Realms;
using IOBootstrap.NET.Common.Entities.Clients;

namespace IOBootstrap.NET.Core.Controllers
{
    public abstract class IOController<TLogger> : Controller
    {

        #region Properties

        public IConfiguration _configuration { get; }
        public IIODatabase _database { get; }
        public ILogger<TLogger> _logger { get; }
        public ILoggerFactory _loggerFactory { get; }

        #endregion

        #region Controller Lifecycle

        public IOController(ILoggerFactory factory, ILogger<TLogger> logger, IConfiguration configuration, IIODatabase database)
        {
            // Setup properties
            _configuration = configuration;
            _database = database;
            _logger = logger;
            _loggerFactory = factory;

            _logger.LogDebug("Request start: {0}", this.GetType().Name);
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            // Check authorization
            if (!this.CheckAuthorizationHeader())
            {
                // Obtain response model
                IOResponseModel responseModel = this.Error400("Authorization failed.");

                // Override response
                context.Result = new JsonResult(responseModel);

                // Do nothing
                return;
            }

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext context) {
            // Dispose database
            _database.Dispose();

            base.OnActionExecuted(context);
        }

        #endregion

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

        public virtual IOResponseModel Error400(string errorMessage = "")
        {
            // Update response status code
            this.Response.StatusCode = 400;

            // Create response status model
            IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, errorMessage);

            // Return response
            return new IOResponseModel(responseStatus);
        }

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

        #region Helper Methods

        private bool CheckAuthorizationHeader()
        {
            // Check authorization header key exists
            if (this.Request.Headers.ContainsKey("X-IO-AUTHORIZATION"))
            {
                // Obtain request authorization value
                string requestAuthorization = this.Request.Headers["X-IO-AUTHORIZATION"];

                // Check authorization code is equal to configuration value
                if (requestAuthorization.Equals(_configuration.GetValue<string>("IOAuthorization")))
                {
                    // Then authorization success
                    return true;
                }

            }

            return false;
        }

        public bool CheckClient(IOClientInfoModel clientInfo) {
            // Obtain realm instance
            Realm realm = _database.GetRealmForMainThread();

            // Find client
            var clientsEntity = realm.All<IOClientsEntity>().Where((arg1) => arg1.ClientId == clientInfo.ClientID);

            // Check finded client counts is greater than zero
            if (clientsEntity.Count() > 0) {
                // Obtain client
                IOClientsEntity client = clientsEntity.First();

                // Check client secret
                if (client.ClientSecret == clientInfo.ClientSecret) {
                    // Then return client valid
                    return true;
                }
            }

            // Then return invalid clients
            return false;
        }

        private string GetControllerName()
        {
            // Obtain class name
            String className = this.GetType().Name;

            // Substract controller word
            string[] controllerName = className.Split(new string[] { "Controller" }, StringSplitOptions.None);

            return controllerName.First();
        }

        #endregion

    }
}
