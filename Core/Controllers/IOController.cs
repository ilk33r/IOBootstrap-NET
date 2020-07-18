using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace IOBootstrap.NET.Core.Controllers
{
    public abstract class IOController<TViewModel, TDBContext> : Controller where TViewModel : IOViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Properties

        public bool ActionExecuted = false;
        public IConfiguration Configuration { get; set; }
        public TDBContext DatabaseContext { get; }
        public IWebHostEnvironment Environment { get; }
        public TViewModel ViewModel { get; }

        #endregion

        #region Controller Lifecycle

        public IOController(IConfiguration configuration, 
                            TDBContext databaseContext,
                            IWebHostEnvironment environment)
        {
            // Setup properties
            Configuration = configuration;
            DatabaseContext = databaseContext;
            Environment = environment;

            // Initialize view model
            ViewModel = new TViewModel();

            // Setup view model properties
            ViewModel.Configuration = configuration;
            ViewModel.DatabaseContext = databaseContext;
            ViewModel.Environment = environment;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Update allow origin
            String allowedOrigins = Configuration.GetValue<string>(IOConfigurationConstants.AllowedOrigin);
            String requestOrigin = Request.Headers["Origin"];
            if (requestOrigin != null && allowedOrigins.Contains(requestOrigin)) {
                Response.Headers.Add("Access-Control-Allow-Origin", requestOrigin);
                Response.Headers.Add("Access-Control-Allow-Headers", Request.Headers["Access-Control-Request-Headers"]);
            }

            // Check request method is options
            if (Request.Method.Equals("OPTIONS")) {
                // Create response status model
                IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.OK);

                // Return response
                JsonResult result = new JsonResult(new IOResponseModel(responseStatus));
                result.StatusCode = 200;
                context.Result = result;

                // Do nothing
                ActionExecuted = true;
                return;
            }

            // Check https is required
            if (checkHttpsRequired(context))
            {
                // Do nothing
                ActionExecuted = true;
                return;
            }

            // Check user role
            if (!CheckRole(context))
            {
                // Do nothing
                ActionExecuted = true;
                return;
            }

            // Update view model request value
            ViewModel.Request = Request;

            // Check authorization
            if (!ViewModel.CheckAuthorizationHeader())
            {
                // Obtain response model
                IOResponseModel responseModel = Error400("Authorization failed.");

                // Override response
                JsonResult result = new JsonResult(responseModel);
                result.StatusCode = 400;
                context.Result = result;

                // Do nothing
                ActionExecuted = true;
                return;
            }

            // Obtain client info
            bool checkClientInfo = Configuration.GetValue<bool>(IOConfigurationConstants.CheckClientInfo);
            if (checkClientInfo) 
            {
                string clientId = (Request.Headers.ContainsKey(IORequestHeaderConstants.ClientId)) ? (string)Request.Headers[IORequestHeaderConstants.ClientId] : "";
                string clientSecret = (Request.Headers.ContainsKey(IORequestHeaderConstants.ClientSecret)) ? (string)Request.Headers[IORequestHeaderConstants.ClientSecret] : "";

                // Check client info
                if (!ViewModel.CheckClient(clientId, clientSecret))
                {
                    // Obtain response model
                    IOResponseModel responseModel = Error400("Invalid client.");

                    // Override response
                    JsonResult result = new JsonResult(responseModel);
                    result.StatusCode = 400;
                    context.Result = result;

                    // Do nothing
                    ActionExecuted = true;
                    return;
                }
            }

            // Check back office page host name
            string backofficePageHostName = Configuration.GetValue<string>(IOConfigurationConstants.BackofficePageHostName);

            // Check hostname is back office page
            if (backofficePageHostName.Equals(Request.Host.Host))
            {
                bool httpsRequired = Configuration.GetValue<bool>(IOConfigurationConstants.HttpsRequired);

                // Check attribute type
                if (httpsRequired && !Request.Scheme.Equals("https")) {
                    // Obtain response model
                    IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.OK, "");
                    IOResponseModel responseModel = new IOResponseModel(responseStatus);
                    JsonResult result = new JsonResult(responseModel);
                    Response.Headers.Add("Location", "https://" + Request.Host.Host);
                    result.StatusCode = 301;
                    context.Result = result;
                    return;
                }

                // Obtain layout name from configuration
                string layoutName = Configuration.GetValue<string>(IOConfigurationConstants.BackofficePageIndexLayoutName);

                // Obtain index page
                context.Result = GetWebIndex(layoutName);

                // Do nothing
                ActionExecuted = true;
                return;
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context) {
            base.OnActionExecuted(context);
        }

        #endregion

        #region Errors

        public virtual IOResponseModel Error400(string errorMessage = "")
        {
            // Update response status code
            Response.StatusCode = 400;

            // Create response status model
            IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.BAD_REQUEST, errorMessage);

            // Return response
            return new IOResponseModel(responseStatus);
        }

        public virtual IOResponseModel Error404()
        {
            // Update response status code
            Response.StatusCode = 404;

            // Create response status model
            IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.ENDPOINT_FAILURE,
                                                                         String.Format("Path {0} is invalid.", Request.Path));

            // Return response
            return new IOResponseModel(responseStatus);
        }

        #endregion

        #region Web Page

        public virtual IActionResult GetWebIndex(string layoutName)
        {
            // Obtain web root
            string webRoot = Environment.WebRootPath;

            // Create file path
            string layoutPath = webRoot + "/" + layoutName;

            if (System.IO.File.Exists(layoutPath))
            {
                // Read file
                string fileContent = System.IO.File.ReadAllText(layoutPath);

                // Obtain web values
                Hashtable webValues = GetWebValues(layoutName);

                // Create html content
                string htmlContent = fileContent;

                // Loop throught web values
                foreach (DictionaryEntry webValue in webValues) 
                {
                    // Set values to content
                    string contentKey = string.Format("${{{0}}}", webValue.Key);
                    htmlContent = htmlContent.Replace(contentKey, (string)webValue.Value);
                }

                return Content(htmlContent, "text/html");
            }
            else
            {
                JsonResult result = new JsonResult(Error404());
                result.StatusCode = 400;
                return result;
            }
        }

        public virtual Hashtable GetWebValues(string layoutName)
        {
            Hashtable webValues = new Hashtable();
            webValues.Add("environmentName", Environment.EnvironmentName);

            // Obtain app version
            string appVersion = Configuration.GetValue<string>(IOConfigurationConstants.Version);
            webValues.Add("version", appVersion);

            return webValues;
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
                        int userRole = ViewModel.GetUserRole();

                        // Check attribute type
                        if (requiredRole != null)
                        {
                            // Check role
                            if (!IOUserRoleUtility.CheckRawRole((int)requiredRole, userRole))
                            {
                                // Obtain response model
                                IOResponseModel responseModel = Error400("Restricted page. User role is " + requiredRole + " required role is " + requiredRole);

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
                        bool httpsRequired = Configuration.GetValue<bool>(IOConfigurationConstants.HttpsRequired);

                        // Check attribute type
                        if (httpsRequired && !Request.Scheme.Equals("https"))
                        {
                            // Obtain response model
                            IOResponseModel responseModel = Error400("Https required.");

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
            String className = GetType().Name;

            // Substract controller word
            string[] controllerName = className.Split(new string[] { "Controller" }, StringSplitOptions.None);

            return controllerName.First();
        }

        #endregion

    }
}
