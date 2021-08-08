using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Core.Logger;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IOBootstrap.NET.Core.Middlewares
{
    public class IOErrorHandlingMiddleware
    {

        private readonly bool IsDevelopment;
        private readonly ILogger<IOLoggerType> Logger;
        private readonly RequestDelegate RequestDelegate;

        public IOErrorHandlingMiddleware(RequestDelegate next, ILogger<IOLoggerType> logger, IWebHostEnvironment env)
        {
            RequestDelegate = next;
            Logger = logger;
            IsDevelopment = !(env.IsProduction());
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await RequestDelegate(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            IOResponseStatusModel responseStatusModel;

            if (ex is IOServiceException)
            {
                IOServiceException serviceException = (IOServiceException)ex;
                responseStatusModel = new IOResponseStatusModel(serviceException.Code, serviceException.Message, false, serviceException.DetailedMessage);
            }
            else if (IsDevelopment) 
            {
                string exceptionContent = ex.Message + '\n' + '\n' + ex.StackTrace;
                responseStatusModel = new IOResponseStatusModel(IOResponseStatusMessages.UnkownException, exceptionContent);

                // Log call
                Logger.LogError(ex, exceptionContent);
            }
            else
            {
                responseStatusModel = new IOResponseStatusModel(IOResponseStatusMessages.UnkownException, "An exception occured.");

                // Log call
                Logger.LogError(ex, ex.Message + '\n' + '\n' + ex.StackTrace);
            }

            IOResponseModel responseModel = new IOResponseModel(responseStatusModel);

            // Override response
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "application/json";

            string responseString = JsonSerializer.Serialize(responseModel, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            await context.Response.WriteAsync(responseString);
        }
    }
}
