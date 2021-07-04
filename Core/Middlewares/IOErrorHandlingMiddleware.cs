using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using IOBootstrap.NET.Common.Constants;
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
            string exceptionContent;

            if (IsDevelopment) 
            {
                exceptionContent = ex.Message + '\n' + '\n' + ex.StackTrace;
            }
            else
            {
                exceptionContent = "An exception occured.";
            }

            IOResponseStatusModel responseStatusModel = new IOResponseStatusModel(IOResponseStatusMessages.GENERAL_EXCEPTION, exceptionContent);
            IOResponseModel responseModel = new IOResponseModel(responseStatusModel);

            // Log call
            Logger.LogError(ex, exceptionContent);

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
