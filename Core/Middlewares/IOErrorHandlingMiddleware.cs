using System;
using System.Net;
using System.Text.Json;
using IOBootstrap.NET.Common.Constants;
using IOBootstrap.NET.Common.Exceptions.Base;
using IOBootstrap.NET.Common.Messages.Base;
using IOBootstrap.NET.Common.Models.Shared;
using IOBootstrap.NET.Common.Logger;
using IOBootstrap.NET.DataAccess.Context;
using IOBootstrap.NET.DataAccess.Entities;
using System.Text;
using IOBootstrap.NET.Common.Utilities;

namespace IOBootstrap.NET.Core.Middlewares;

public class IOErrorHandlingMiddleware<TDBContext>
where TDBContext : IOBaseDatabaseContext<TDBContext>
{

    private readonly ILogger<IOLoggerType> Logger;
    private readonly RequestDelegate RequestDelegate;
    private readonly IServiceScopeFactory ServiceScopeFactory;

    public IOErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<IOLoggerType> logger,
        IServiceScopeFactory serviceScopeFactory
    )
    {
        RequestDelegate = next;
        Logger = logger;
        ServiceScopeFactory = serviceScopeFactory;
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
        else
        {
#if DEBUG
            string exceptionContent = ex.Message + '\n' + '\n' + ex.StackTrace;
            responseStatusModel = new IOResponseStatusModel(IOResponseStatusMessages.UnkownException, exceptionContent);

            // Log call
            Logger.LogError(ex, exceptionContent);
#else
            responseStatusModel = new IOResponseStatusModel(IOResponseStatusMessages.UnkownException, "An exception occured.");

            // Log call
            Logger.LogError(ex, ex.Message + '\n' + '\n' + ex.StackTrace);
#endif

            using (IServiceScope scope = ServiceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TDBContext>();
                await SaveExceptionToDatabase(context, dbContext, ex);
            }
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

    private async Task SaveExceptionToDatabase(HttpContext context, TDBContext dbContext, Exception ex)
    {
        IOExceptionEntity exception = await CreateExceptionEntity(context, ex);

        int stackTraceLength = ex.StackTrace?.Count() ?? 0;
        int stackTraceBuffer = 0;
        while (stackTraceBuffer < stackTraceLength)
        {
            IOExceptionEntity copiedException = IOSerializableUtilities.Copy(exception);

            string exceptionStackTrace = string.Empty;
            int stringSize = Math.Min(stackTraceLength - stackTraceBuffer, 2048);
            if (!String.IsNullOrEmpty(ex.StackTrace))
            {
                exceptionStackTrace = ex.StackTrace?.Substring(stackTraceBuffer, stringSize) ?? string.Empty;
            }

            copiedException.ExceptionStackTrace = exceptionStackTrace;
            dbContext.Add(copiedException);

            stackTraceBuffer += stringSize;
        }

        dbContext.SaveChanges();
    }
    
    private async Task<IOExceptionEntity> CreateExceptionEntity(HttpContext context, Exception ex)
    {
        string requestPath = context.Request.Path.ToString();
        string requestHeadersJson = JsonSerializer.Serialize(context.Request.Headers);

        string? requestBody;
        if (context.Request?.Method.Equals("GET") ?? true)
        {
            requestBody = context.Request?.QueryString.ToString();
        }
        else if (context.Request?.ContentType?.ToLower().Contains("application/json") ?? false)
        {
            context.Request.Body.Seek(0, SeekOrigin.Begin);
            using (StreamReader requestBodyReader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, leaveOpen: true))
            {
                string currentRequestBody = await requestBodyReader.ReadToEndAsync();
                requestBody = currentRequestBody.Substring(0, Math.Min(currentRequestBody.Length, 2048));
            }
            context.Request.Body.Seek(0, SeekOrigin.Begin);
        }
        else
        {
            requestBody = null;
        }

        if (!String.IsNullOrEmpty(requestPath))
        {
            requestPath = requestPath.Substring(0, Math.Min(requestPath.Length, 64));
        }

        if (!String.IsNullOrEmpty(requestHeadersJson))
        {
            requestHeadersJson = requestHeadersJson.Substring(0, Math.Min(requestHeadersJson.Length, 512));
        }

        string exceptionMessage = string.Empty;
        if (!String.IsNullOrEmpty(ex.Message))
        {
            exceptionMessage = ex.Message.Substring(0, Math.Min(ex.Message.Length, 2048));
        }

        string exceptionStackTrace = string.Empty;

        return new IOExceptionEntity()
        {
            RequestDate = DateTimeOffset.UtcNow,
            RequestPath = requestPath,
            RequestHeaders = requestHeadersJson,
            RequestBody = requestBody,
            ExceptionMessage = exceptionMessage,
            ExceptionStackTrace = exceptionStackTrace,
        };
    }
}