using System;
using System.Reflection;
using System.Text.Json;
using IOBootstrap.Net.Common.MWConnector;
using IOBootstrap.NET.Common.Attributes;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Configuration;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IOBootstrap.NET.Core.Controllers
{
    public abstract class IOController<TViewModel> : Controller where TViewModel : IOViewModel, new()
    {

        #region Properties

        public IConfiguration Configuration { get; set; }
        public IWebHostEnvironment Environment { get; }
        public ILogger<IOLoggerType> Logger { get; }
        public TViewModel ViewModel { get; }
        public bool IsBackofficePage;

        #endregion

        #region Controller Lifecycle

        public IOController(IConfiguration configuration, 
                            IWebHostEnvironment environment,
                            ILogger<IOLoggerType> logger)
        {
            // Setup properties
            Configuration = configuration;
            Environment = environment;
            Logger = logger;
            IsBackofficePage = false;

            // Initialize view model
            ViewModel = new TViewModel();

            // Setup view model properties
            ViewModel.Configuration = configuration;
            ViewModel.Environment = environment;
            ViewModel.Logger = logger;
            ViewModel.MWConnector = new IOMWConnector(logger, configuration);
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
                    IOResponseModel responseModel = new IOResponseModel();
                    JsonResult result = new JsonResult(responseModel);
                    Response.Headers.Add("Location", "https://" + Request.Host.Host);
                    result.StatusCode = 301;
                    context.Result = result;
                    IsBackofficePage = true;
                    return;
                }

                // Obtain layout name from configuration
                string layoutName = Configuration.GetValue<string>(IOConfigurationConstants.BackofficePageIndexLayoutName);
                IsBackofficePage = true;
                
                // Obtain index page
                context.Result = GetWebIndex(layoutName);

                // Do nothing
                return;
            }

            // Check maintenance status
            CheckIsMaintenanceMode(context);

            // Validate request
            ValidateRequestModel(context);
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
                ContentResult contentResult = Content(fileContent, "text/html");
                contentResult.StatusCode = 200;
                return contentResult;
            }
            else
            {
                JsonResult result = new JsonResult(Error404());
                return result;
            }
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

        public virtual void CheckIsMaintenanceMode(ActionExecutingContext context)
        {
            // Obtain action desctriptor
            ControllerActionDescriptor actionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;
            bool isBackofficePage = false;

            if (actionDescriptor != null)
            {
                // Loop throught descriptors
                foreach (CustomAttributeData descriptor in actionDescriptor.ControllerTypeInfo.CustomAttributes)
                {
                    if (descriptor.AttributeType == typeof(IOBackofficeAttribute))
                    {
                        isBackofficePage = true;
                        break;
                    }
                }
            }

            if (!isBackofficePage)
            {
                IOConfigurationModel isMaintenanceModeOn = ViewModel.GetDBConfig(IOConfigurationKeys.IsMaintenanceModeOn);
                if (isMaintenanceModeOn != null && isMaintenanceModeOn.IntValue() == 1)
                {
                    throw new IOMaintenanceException();
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

        public virtual void ValidateRequestModel(ActionExecutingContext context)
        {
            if (!HasControllerAttribute<IOValidateRequestModelAttribute>(context))
            {
                return;
            }

            if (ModelState.IsValid)
            {
                return;
            }

            string detailedMessage = "";
            if (ModelState.Values.Count() > 0)
            {
                ModelStateEntry entry = ModelState.Values.First();
                
                if (entry.Errors.Count() > 0)
                {
                    detailedMessage = entry.Errors.First().ErrorMessage;
                }
            }
            
            throw new IOInvalidRequestException(detailedMessage);
        }

        #endregion

    }
}
