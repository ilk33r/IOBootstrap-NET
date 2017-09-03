using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Core.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace IOBootstrap.NET.Core.Controllers
{
    public abstract class IOController<TLogger, TViewModel> : Controller 
        where TViewModel: IOViewModel, new()
    {

        #region Properties

        public IConfiguration _configuration { get; }
        public IIODatabase _database { get; }
        public IHostingEnvironment _environment { get; }
        public ILogger<TLogger> _logger { get; }
        public ILoggerFactory _loggerFactory { get; }
        public TViewModel _viewModel { get; }

        #endregion

        #region Controller Lifecycle

        public IOController(ILoggerFactory factory, ILogger<TLogger> logger, 
                            IConfiguration configuration, 
                            IIODatabase database,
                            IHostingEnvironment environment)
        {
            // Setup properties
            _configuration = configuration;
            _database = database;
            _environment = environment;
            _logger = logger;
            _loggerFactory = factory;

            // Initialize view model
            _viewModel = new TViewModel();

            // Setup view model properties
            _viewModel.Configuration = configuration;
            _viewModel.Database = database;
            _viewModel.Environment = environment;
            _viewModel.Logger = logger;

            _logger.LogDebug("Request start: {0}", this.GetType().Name);
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Update view model request value
            _viewModel.Request = this.Request;

            // Check authorization
            if (!_viewModel.CheckAuthorizationHeader())
            {
                // Obtain response model
                IOResponseModel responseModel = this.Error400("Authorization failed.");

                // Override response
                context.Result = new JsonResult(responseModel);

                // Do nothing
                return;
            }
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

        public string GetControllerName()
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
