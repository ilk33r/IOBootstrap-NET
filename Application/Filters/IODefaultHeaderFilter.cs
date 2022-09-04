using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IOBootstrap.NET.Application.Filters;

public class IODefaultHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        IOCacheObject authorizationCache = IOCache.GetCachedObject(IOCacheKeys.SwaggerAuthorization);
        string authorization = "";
        if (authorizationCache != null)
        {
            authorization = (string)authorizationCache.Value;
        }

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-IO-AUTHORIZATION",
            In = ParameterLocation.Header,
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString(authorization)
            }
        });

        string keyID = "";
        IOCacheObject keyIDCacheObject = IOCache.GetCachedObject(IOCacheKeys.RSAPrivateKeyIDCacheKey);
        if (keyIDCacheObject != null) 
        {
            keyID = (string)keyIDCacheObject.Value;
        }
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-KEY-ID",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString(keyID)
            }
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-SYMMETRIC-KEY",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString("")
            }
        });

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-SYMMETRIC-IV",
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString("")
            }
        });
    }
}
