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
    public abstract class IOController<TLogger, TViewModel, TDBContext> : Controller 
        where TViewModel : IOViewModel<TDBContext>, new() 
        where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Properties

        public IConfiguration _configuration { get; }
        public TDBContext _databaseContext { get; }
        public IHostingEnvironment _environment { get; }
        public ILogger<TLogger> _logger { get; }
        public ILoggerFactory _loggerFactory { get; }
        public TViewModel _viewModel { get; }

        #endregion

        #region Controller Lifecycle

        public IOController(ILoggerFactory factory, ILogger<TLogger> logger, 
                            IConfiguration configuration, 
                            TDBContext databaseContext,
                            IHostingEnvironment environment)
        {
            // Setup properties
            _configuration = configuration;
            _databaseContext = databaseContext;
            _environment = environment;
            _logger = logger;
            _loggerFactory = factory;

            // Initialize view model
            _viewModel = new TViewModel();

            // Setup view model properties
            _viewModel._configuration = configuration;
            _viewModel._databaseContext = databaseContext;
            _viewModel._environment = environment;
            _viewModel._logger = logger;

            _logger.LogDebug("Request start: {0}", this.GetType().Name);
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Update allow origin
            Response.Headers.Add("Access-Control-Allow-Origin", _configuration.GetValue<string>("IOAllowedOrigin"));
            Response.Headers.Add("Access-Control-Allow-Headers", Request.Headers["Access-Control-Request-Headers"]);

            // Check request method is options
            if (this.Request.Method.Equals("OPTIONS")) {
                // Create response status model
                IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.OK);

                // Return response
                context.Result = new JsonResult(new IOResponseModel(responseStatus));

                // Do nothing
                return;
            }

            // Update view model request value
            _viewModel._request = this.Request;

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

            // Obtain client info
            bool checkClientInfo = _configuration.GetValue<bool>("IOCheckClientInfo");

            if (checkClientInfo) 
            {
                string clientId = (Request.Headers.ContainsKey("X-IO-CLIENT-ID")) ? (string)Request.Headers["X-IO-CLIENT-ID"] : "";
                string clientSecret = (Request.Headers.ContainsKey("X-IO-CLIENT-SECRET")) ? (string)Request.Headers["X-IO-CLIENT-SECRET"] : "";

                // Check client info
                if (!_viewModel.CheckClient(clientId, clientSecret))
                {
                    // Obtain response model
                    IOResponseModel responseModel = this.Error400("Invalid client.");

                    // Override response
                    context.Result = new JsonResult(responseModel);

                    // Do nothing
                    return;
                }
            }

            // Check back office page host name
            string backofficePageHostName = this._configuration.GetValue<string>("IOBackofficePageHostName");

            // Check hostname is back office page
            if (backofficePageHostName.Equals(this.Request.Host.Host))
            {
                // Obtain layout name from configuration
                string layoutName = this._configuration.GetValue<string>("IOBackofficePageIndexLayoutName");

                // Obtain index page
                context.Result = this.GetWebIndex(layoutName);

                // Do nothing
                return;
            }
        }

        public override void OnActionExecuted(Microsoft.AspNetCore.Mvc.Filters.ActionExecutedContext context) {
            base.OnActionExecuted(context);
        }

        #endregion

        #region Default

        public virtual IOResponseModel Index()
        {
            // Obtain app version
            string appVersion = _configuration.GetValue<string>("IOVersion");

            // Create response status model
            IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.OK,
                                                                             "IO Bootstrapt.",
                                                                             true,
                                                                             String.Format("Version: {0}", appVersion));

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

        #region Web Page

        public virtual IActionResult GetWebIndex(string layoutName)
        {
            // Obtain web root
            string webRoot = this._environment.WebRootPath;

            // Create file path
            string layoutPath = webRoot + "/" + layoutName;

            if (System.IO.File.Exists(layoutPath))
            {
                // Read file
                string fileContent = System.IO.File.ReadAllText(layoutPath);

                // Obtain app version
                string appVersion = _configuration.GetValue<string>("IOVersion");

                // Set values to content
                string htmlContent = fileContent.Replace("${environmentName}", _environment.EnvironmentName)
                                                .Replace("${version}", appVersion);

                return Content(htmlContent, "text/html");
            }
            else
            {
                return new JsonResult(this.Error404());
            }
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
