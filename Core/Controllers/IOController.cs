using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Common;
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

        [Obsolete("This Property is Deprecated", false)]
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
            Logger = logger;

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
            CheckHttpsRequired(context);

            // Check user role
            CheckRole(context);

            // Update view model request value
            ViewModel.Request = Request;

            // Check authorization
            ViewModel.CheckAuthorizationHeader();

            // Check key id
            CheckKeyID(context);

            // Check client info
            ViewModel.CheckClient();

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

            // Check result type
            string jsonString = null;
            if (context.Result is JsonResult)
            {
                // Create JSON string
                JsonResult resultJson = (JsonResult)context.Result;
                jsonString = JsonSerializer.Serialize(resultJson.Value);    
            }
            else if (context.Result is ObjectResult)
            {
                // Create JSON string
                ObjectResult resultJson = (ObjectResult)context.Result;
                jsonString = JsonSerializer.Serialize(resultJson.Value);    
            }

            if (jsonString != null)
            {
                // Log call
                Logger.LogInformation(String.Format("{0} - {1}", Request.Path, jsonString));
            }
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
            IOResponseStatusModel responseStatus = new IOResponseStatusModel(IOResponseStatusMessages.EndpointFailure,
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
            string backofficePagePath = Configuration.GetValue<string>(IOConfigurationConstants.BackofficePagePath);
            webValues.Add("backOfficePageURL", backOfficePageURL);
            webValues.Add("backOfficePagePath", backofficePagePath);

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
            string pushNotificationsControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficePushNotificationControllerNameKey);
            string resourcesControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackOfficeResourcesControllerNameKey);
            string userControllerName = Configuration.GetValue<string>(IOConfigurationConstants.BackofficeUserControllerNameKey);
            webValues.Add("authenticationControllerName", authenticationControllerName);
            webValues.Add("backOfficeControllerName", backOfficeControllerName);
            webValues.Add("configurationControllerName", configurationControllerName);
            webValues.Add("imagesControllerName", imagesControllerName);
            webValues.Add("menuControllerName", menuControllerName);
            webValues.Add("messagesControllerName", messagesControllerName);
            webValues.Add("pushNotificationsControllerName", pushNotificationsControllerName);
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

        private void CheckKeyID(ActionExecutingContext context)
        {
            // Obtain key id
            string keyID = Request.Headers[IORequestHeaderConstants.KeyID];
            if (String.IsNullOrEmpty(keyID))
            {
                return;
            }

            string currentKeyID = "";

            // Obtain key id cache
            IOCacheObject keyIDCache = IOCache.GetCachedObject(IOCacheKeys.RSAPrivateKeyIDCacheKey);
            if (keyIDCache != null)
            {
                currentKeyID = (string)keyIDCache.Value;
            }

            if (currentKeyID.Equals(keyID))
            {
                return;
            }

            throw new IOInvalidKeyIDException();
        }

        public virtual void CheckRole(ActionExecutingContext context)
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
                            throw new IOInvalidPermissionException("Restricted page. User role is " + userRole + " required role is " + requiredRole);
                        }
                    }
                }
            }
        }

        public virtual void CheckHttpsRequired(ActionExecutingContext context)
        {   
            if (HasControllerAttribute<IORequireHTTPSAttribute>(context))
            {
                bool httpsRequired = Configuration.GetValue<bool>(IOConfigurationConstants.HttpsRequired);

                // Check attribute type
                if (httpsRequired && !Request.Scheme.Equals("https"))
                {
                    throw new IOHttpsRequiredException();
                }
            }
        }

        public virtual string GetControllerName()
        {
            // Obtain class name
            String className = GetType().Name;

            // Substract controller word
            string[] controllerName = className.Split(new string[] { "Controller" }, StringSplitOptions.None);

            return controllerName.First();
        }

        public virtual bool HasControllerAttribute<T>(ActionExecutingContext context)
        {
            // Obtain action desctriptor
            ControllerActionDescriptor actionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;

            if (actionDescriptor != null)
            {
                // Loop throught descriptors
                foreach (CustomAttributeData descriptor in actionDescriptor.MethodInfo.CustomAttributes)
                {
                    if (descriptor.AttributeType == typeof(T))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

    }
}
