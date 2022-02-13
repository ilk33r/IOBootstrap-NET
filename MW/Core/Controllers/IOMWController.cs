using System;

namespace IOBootstrap.NET.MW.Core.Controllers
{
    public class IOMWController<TViewModel, TDBContext> : Controller where TViewModel : IOMWViewModel<TDBContext>, new() where TDBContext : IODatabaseContext<TDBContext>
    {
        #region Properties

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
            IsBackofficePage = false;

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

            // Update view model request value
            ViewModel.Request = Request;

            // Check authorization
            ViewModel.CheckAuthorizationHeader();
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

        #region Helper Methods

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
