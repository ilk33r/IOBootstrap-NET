using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.Logger;
using IOBootstrap.NET.Core.ViewModels;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Core.Controllers
{
    public abstract class IOController<TViewModel, TDBContext> : Controller where TViewModel : IOViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {

        #region Properties

        public bool ActionExecuted = false;
        public IConfiguration Configuration { get; set; }
        public TDBContext DatabaseContext { get; }
        public IWebHostEnvironment Environment { get; }
        public ILogger<IOLoggerType> Logger { get; }
        public TViewModel ViewModel { get; }

        #endregion

        #region Controller Lifecycle

        public IOController(IConfiguration configuration, 
                            TDBContext databaseContext,
                            IWebHostEnvironment environment,
                            ILogger<IOLoggerType> logger)
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
            ViewModel.Logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Check https is required
            if (CheckHttpsRequired(context))
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
                IOResponseModel responseModel = new IOResponseModel(IOResponseStatusMessages.AUTHORIZATION_FAILED);

                // Override response
                JsonResult result = new JsonResult(responseModel);
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
                    IOResponseModel responseModel = new IOResponseModel(IOResponseStatusMessages.INVALID_CLIENT);

                    // Override response
                    JsonResult result = new JsonResult(responseModel);
                    context.Result = result;

                    // Do nothing
                    ActionExecuted = true;
                    return;
                }
            }

            // Check back office page host name
            string backofficePageHostName = Configuration.GetValue<string>(IOConfigurationConstants.BackofficePageHostName);
            string backofficePagePath = Configuration.GetValue<string>(IOConfigurationConstants.BackofficePagePath);
            string requestPath = Request.Path;
            if (HttpContext.Items.ContainsKey("OriginalPath"))
            {
                requestPath = (string)HttpContext.Items["OriginalPath"];
            }
            
            bool isBackofficePath = (String.IsNullOrEmpty(requestPath) || requestPath.Contains(backofficePagePath));

            // Check hostname is back office page
            if (backofficePageHostName.Equals(Request.Host.Host) && isBackofficePath)
            {
                bool httpsRequired = Configuration.GetValue<bool>(IOConfigurationConstants.HttpsRequired);

                // Check attribute type
                if (httpsRequired && !Request.Scheme.Equals("https")) {
                    // Obtain response model
                    IOResponseModel responseModel = new IOResponseModel(IOResponseStatusMessages.OK);
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

        public virtual IOResponseModel Error404()
        {
            // Update response status code
            Response.StatusCode = 200;

            // Obtain request path
            string requestPath = Request.Path;

            if (HttpContext.Items.ContainsKey("OriginalPath"))
            {
                requestPath = (string)HttpContext.Items["OriginalPath"];
            }

            // Create response status model
            IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.ENDPOINT_FAILURE,
                                                                         String.Format("Path {0} is invalid.", requestPath));

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

            // Obtain backoffice page url
            string backOfficePageURL = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficePageURLKey);
            webValues.Add("backOfficePageURL", backOfficePageURL);

            // Obtain api url and app name
            string apiURL = Configuration.GetValue<string>(IOConfigurationConstants.APIURLKey);
            string appName = Configuration.GetValue<string>(IOConfigurationConstants.APPNameKey);
            webValues.Add("apiURL", apiURL);
            webValues.Add("appName", appName);

            // Obtain authorization
            string authorization = Configuration.GetValue<string>(IOConfigurationConstants.AuthorizationKey);
            webValues.Add("authorization", authorization);

            // Obtain client id and secret
            string backofficeClientID = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeClientIDKey);
            string backofficeClientSecret = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeClientSecretKey);
            webValues.Add("clientID", backofficeClientID);
            webValues.Add("clientSecret", backofficeClientSecret);

            // Obtain controllers
            string authenticationControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeAuthenticationControllerNameKey);
            string backOfficeControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeControllerNameKey);
            string configurationControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeConfigurationControllerNameKey);
            string imagesControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeImagesControllerNameKey);
            string menuControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMenuControllerNameKey);
            string messagesControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeMessagesControllerNameKey);
            string resourcesControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeResourcesControllerNameKey);
            string userControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackofficeUserControllerNameKey);
            webValues.Add("authenticationControllerName", authenticationControllerName);
            webValues.Add("backOfficeControllerName", backOfficeControllerName);
            webValues.Add("configurationControllerName", configurationControllerName);
            webValues.Add("imagesControllerName", imagesControllerName);
            webValues.Add("menuControllerName", menuControllerName);
            webValues.Add("messagesControllerName", messagesControllerName);
            webValues.Add("resourcesControllerName", resourcesControllerName);
            webValues.Add("userControllerName", userControllerName);

            string storageBaseURL = Configuration.GetValue<string>(IOConfigurationConstants.StorageBaseURLKey);
            string storageBlobName = Configuration.GetValue<string>(IOConfigurationConstants.AzureStorageBlobNameKey);
            string storageBaseURLWithBlobName = storageBaseURL + storageBlobName + "/";
            webValues.Add("storageBaseUrl", storageBaseURLWithBlobName);

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

                        // Check attribute type and role
                        if (requiredRole != null && !IOUserRoleUtility.CheckRawRole((int)requiredRole, userRole))
                        {
                            // Obtain response model
                            IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.INVALID_PERMISSION, "Restricted page. User role is " + userRole + " required role is " + requiredRole);
                            IOResponseModel responseModel = new IOResponseModel(responseStatus);

                            // Override response
                            JsonResult result = new JsonResult(responseModel);
                            context.Result = result;

                            // Do nothing
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool CheckHttpsRequired(ActionExecutingContext context)
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
                            IOResponseModel responseModel = new IOResponseModel(IOResponseStatusMessages.HTTPS_REQUIRED);

                            // Override response
                            JsonResult result = new JsonResult(responseModel);
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
