using System;
using System.Text.Json;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Common;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Core.Interfaces;
using IOBootstrap.NET.DataAccess.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IOBootstrap.NET.Core.Controllers
{
    public abstract class IOFunctionController<TViewModel, TDBContext> : IOController<TViewModel, TDBContext>
    where TDBContext : IODatabaseContext<TDBContext> 
    where TViewModel : IIOFunctionsViewModel<TDBContext>, new()
    {

        #region Controller Lifecycle

        public IOFunctionController(IConfiguration configuration, 
                                    IWebHostEnvironment environment,
                                    ILogger<IOLoggerType> logger,
                                    TDBContext databaseContext) : base(configuration, environment, logger, databaseContext)
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Validate request
            CheckRequestIsEncrypted(context);
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

                // Check encryption is enabled
                string encryptedResult = ViewModel.EncryptResult(jsonString);
                ContentResult contentResult = Content(encryptedResult, "text/plain");
                contentResult.StatusCode = 200;

                context.HttpContext.Response.Headers.Add(IORequestHeaderConstants.IsEncrypted, "true");
                context.Result = contentResult;
            }

            if (jsonString != null)
            {
                // Log call
                Logger.LogInformation(String.Format("{0} - {1}", Request.Path, jsonString));
            }
        }

        #endregion

        #region Helper Methods

        [ApiExplorerSettings(IgnoreApi = true)]
        public virtual void CheckRequestIsEncrypted(ActionExecutingContext context)
        {   
            // Check attribute type
            if (!Request.Headers.ContainsKey(IORequestHeaderConstants.IsEncrypted))
            {
                throw new IOInvalidRequestException();
            }

            // Obtain token
            string isEncrypted = Request.Headers[IORequestHeaderConstants.IsEncrypted];
            if (!isEncrypted.Equals("true"))
            {
                throw new IOInvalidRequestException();
            }
        }

        #endregion
    }
}
