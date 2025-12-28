using System;
using System.Text;
using System.Text.Json;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.Common.Utilities;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IOBootstrap.NET.Application.Filters;

public class IODatabaseLogFilter<TDBContext> : IAsyncActionFilter
where TDBContext : IOBaseDatabaseContext<TDBContext>
{
    private readonly IConfiguration Configuration;
    private readonly ILogger<IOLoggerType> Logger;
    private readonly IServiceScopeFactory ServiceScopeFactory;

    public IODatabaseLogFilter(
        ILogger<IOLoggerType> logger,
        IWebHostEnvironment env,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        Configuration = configuration;
        Logger = logger;
        ServiceScopeFactory = serviceScopeFactory;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (
            context.HttpContext.Request.Method.Equals("POST") ||
            context.HttpContext.Request.Method.Equals("GET") ||
            context.HttpContext.Request.Method.Equals("DELETE")
        )
        {
            context.HttpContext.Request.EnableBuffering();
            string requestPath = context.HttpContext.Request.Path.ToString();
            string requestHeadersJson = JsonSerializer.Serialize(context.HttpContext.Request.Headers);
            string? ip4;
            int? port;

            if (context.HttpContext.Request != null)
            {
                ip4 = IOHttpUtilities.GetUserIP(context.HttpContext.Request);
                port = IOHttpUtilities.GetUserPort(context.HttpContext.Request);
            }
            else
            {
                ip4 = null;
                port = null;
            }

            string? requestBody;
            if (context.HttpContext.Request?.Method.Equals("GET") ?? true)
            {
                requestBody = context.HttpContext.Request?.QueryString.ToString();
            }
            else if (context.HttpContext.Request?.ContentType?.ToLower().Contains("application/json") ?? false)
            {
                context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                using (StreamReader requestBodyReader = new StreamReader(context.HttpContext.Request.Body, Encoding.UTF8, true, 1024, leaveOpen: true))
                {
                    string currentRequestBody = await requestBodyReader.ReadToEndAsync();
                    requestBody = currentRequestBody.Substring(0, Math.Min(currentRequestBody.Length, 2048));
                }
                context.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                requestBody = null;
            }

            var resultContext = await next();

            string? responseBody;
            if (resultContext.Result is ObjectResult objectResult)
            {
                var responseContent = objectResult.Value;
                responseBody = JsonSerializer.Serialize(responseContent);
                responseBody = responseBody.Substring(0, Math.Min(responseBody.Length, 2048));
            }
            else if (resultContext.Result is ContentResult contentResult)
            {
                responseBody = contentResult.Content;
                responseBody = responseBody?.Substring(0, Math.Min(responseBody?.Length ?? 0, 2048));
            }
            else
            {
                responseBody = null;
            }

            string responseHeadersJson = JsonSerializer.Serialize(resultContext.HttpContext.Response.Headers);

            IOLogsEntity log = new IOLogsEntity()
            {
                RequestDate = DateTimeOffset.UtcNow,
                IPV4 = ip4,
                Port = port,
                ResponseCode = resultContext.HttpContext.Response?.StatusCode,
                RequestPath = requestPath.Substring(0, Math.Min(requestPath.Length, 64)),
                RequestHeaders = requestHeadersJson.Substring(0, Math.Min(requestHeadersJson.Length, 512)),
                ResponseHeaders = responseHeadersJson.Substring(0, Math.Min(responseHeadersJson.Length, 512)),
                RequestBody = requestBody,
                ResponseBody = responseBody
            };

            using (IServiceScope scope = ServiceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TDBContext>();

                // Do some background processing with the EF database context.
                dbContext.Add(log);
                dbContext.SaveChanges();
            }
        }
        else
        {
            await next();
        }
    }
}
