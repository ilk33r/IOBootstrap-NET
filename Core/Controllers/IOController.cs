using System;
using System.Linq;
using System.Reflection;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Models.BaseModels;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Database;
using IOBootstrap.NET.Core.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Core.Controllers
{
    public abstract class IOController<TLogger, TViewModel, TDBContext> : Controller 
        where TViewModel : IOViewModel<TDBContext>, new() 
        where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Properties

        public bool ActionExecuted = false;
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

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Update allow origin
            Response.Headers.Add("Access-Control-Allow-Origin", _configuration.GetValue<string>(IOConfigurationConstants.AllowedOrigin));
            Response.Headers.Add("Access-Control-Allow-Headers", Request.Headers["Access-Control-Request-Headers"]);

            // Check request method is options
            if (this.Request.Method.Equals("OPTIONS")) {
                // Create response status model
                IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.OK);

                // Return response
                JsonResult result = new JsonResult(new IOResponseModel(responseStatus));
                result.StatusCode = 200;
                context.Result = result;

                // Do nothing
                this.ActionExecuted = true;
                return;
            }

            // Check https is required
            if (this.checkHttpsRequired(context))
            {
                // Do nothing
                return;
            }

            // Check user role
            if (!this.CheckRole(context))
            {
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
                JsonResult result = new JsonResult(responseModel);
                result.StatusCode = 400;
                context.Result = result;

                // Do nothing
                this.ActionExecuted = true;
                return;
            }

            // Obtain client info
            bool checkClientInfo = _configuration.GetValue<bool>(IOConfigurationConstants.CheckClientInfo);

            if (checkClientInfo) 
            {
                string clientId = (Request.Headers.ContainsKey(IORequestHeaderConstants.ClientId)) ? (string)Request.Headers[IORequestHeaderConstants.ClientId] : "";
                string clientSecret = (Request.Headers.ContainsKey(IORequestHeaderConstants.ClientSecret)) ? (string)Request.Headers[IORequestHeaderConstants.ClientSecret] : "";

                // Check client info
                if (!_viewModel.CheckClient(clientId, clientSecret))
                {
                    // Obtain response model
                    IOResponseModel responseModel = this.Error400("Invalid client.");

                    // Override response
                    JsonResult result = new JsonResult(responseModel);
                    result.StatusCode = 400;
                    context.Result = result;

                    // Do nothing
                    this.ActionExecuted = true;
                    return;
                }
            }

            // Check back office page host name
            string backofficePageHostName = this._configuration.GetValue<string>(IOConfigurationConstants.BackofficePageHostName);

            // Check hostname is back office page
            if (backofficePageHostName.Equals(this.Request.Host.Host))
            {
                // Obtain layout name from configuration
                string layoutName = this._configuration.GetValue<string>(IOConfigurationConstants.BackofficePageIndexLayoutName);

                // Obtain index page
                context.Result = this.GetWebIndex(layoutName);

                // Do nothing
                this.ActionExecuted = true;
                return;
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context) {
            base.OnActionExecuted(context);
        }

        #endregion

        #region Default

        public virtual IOResponseModel Index()
        {
            // Obtain app version
            string appVersion = _configuration.GetValue<string>(IOConfigurationConstants.Version);

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
                string appVersion = _configuration.GetValue<string>(IOConfigurationConstants.Version);

                // Set values to content
                string htmlContent = fileContent.Replace("${environmentName}", _environment.EnvironmentName)
                                                .Replace("${version}", appVersion);

                return Content(htmlContent, "text/html");
            }
            else
            {
                JsonResult result = new JsonResult(this.Error404());
                result.StatusCode = 400;
                return result;
            }
        }

        #endregion

        #region Helper Methods

        public virtual bool CheckRole(ActionExecutingContext context)
        {
            // Obtain action desctriptor
            ControllerActionDescriptor actionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;

            if (actionDescriptor != null)
            {
                // Loop throught descriptors
                foreach (CustomAttributeData descriptor in actionDescriptor.MethodInfo.CustomAttributes)
                {
                    if (descriptor.AttributeType == typeof(IOUserRoleAttribute) || descriptor.AttributeType == typeof(IOUserCustomRoleAttribute))
                    {
                        object requiredRole = descriptor.ConstructorArguments[0].Value;
                        int userRole = _viewModel.GetUserRole();

                        // Check attribute type
                        if (requiredRole != null)
                        {
                            // Check role
                            if (!IOUserRoleUtility.CheckRawRole((int)requiredRole, userRole))
                            {
                                // Obtain response model
                                IOResponseModel responseModel = this.Error400("Restricted page.");

                                // Override response
                                JsonResult result = new JsonResult(responseModel);
                                result.StatusCode = 400;
                                context.Result = result;

                                // Do nothing
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public bool checkHttpsRequired(ActionExecutingContext context)
        {
            // Obtain action desctriptor
            ControllerActionDescriptor actionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;

            if (actionDescriptor != null)
            {
                // Loop throught descriptors
                foreach (CustomAttributeData descriptor in actionDescriptor.MethodInfo.CustomAttributes)
                {
                    if (descriptor.AttributeType == typeof(IORequireHTTPSAttribute))
                    {
                        bool httpsRequired = _configuration.GetValue<bool>(IOConfigurationConstants.HttpsRequired);

                        // Check attribute type
                        if (httpsRequired && !this.Request.Scheme.Equals("https"))
                        {
                            // Obtain response model
                            IOResponseModel responseModel = this.Error400("Https required.");

                            // Override response
                            JsonResult result = new JsonResult(responseModel);
                            result.StatusCode = 400;
                            context.Result = result;

                            // Do nothing
                            return true;
                        }
                    }
                }
            }

            return false;
        }

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
