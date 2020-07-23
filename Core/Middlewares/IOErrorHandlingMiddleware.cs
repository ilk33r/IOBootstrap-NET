using System;
using System.Net;
using System.Threading.Tasks;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;

namespace IOBootstrap.NET.Core.Middlewares
{
    public class IOErrorHandlingMiddleware
    {

        private readonly RequestDelegate RequestDelegate;

        public IOErrorHandlingMiddleware(RequestDelegate next)
        {
            RequestDelegate = next;
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

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            IOResponseStatusModel responseStatusModel = new IOResponseStatusModel(IOResponseStatusMessages.GENERAL_EXCEPTION, ex.Message + '\t' + '\t' + ex.StackTrace);
            IOResponseModel responseModel = new IOResponseModel(responseStatusModel);

            // Override response
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "application/json";

            string responseString = JsonConvert.SerializeObject(responseModel, new JsonSerializerSettings 
            { 
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() 
            });
            await context.Response.WriteAsync(responseString);
        }
    }
}
