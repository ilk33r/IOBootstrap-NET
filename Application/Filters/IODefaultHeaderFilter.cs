using System.Text.Json.Nodes;
using IOBootstrap.NET.Common.Cache;
using IOBootstrap.NET.Common.Constants;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IOBootstrap.NET.Application.Filters;

public class IODefaultHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        IOCacheObject? authorizationCache = IOCache.GetCachedObject(IOCacheKeys.SwaggerAuthorization);
        string authorization = "";
        if (authorizationCache != null)
        {
            authorization = (string)authorizationCache.Value;
        }

        operation.Parameters?.Add(new OpenApiParameter
        {
            Name = IORequestHeaderConstants.Authorization,
            In = ParameterLocation.Header,
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Default = JsonValue.Create(authorization)
            }
        });

        string keyID = "";
        IOCacheObject? keyIDCacheObject = IOCache.GetCachedObject(IOCacheKeys.RSAPrivateKeyIDCacheKey);
        if (keyIDCacheObject != null) 
        {
            keyID = (string)keyIDCacheObject.Value;
        }
        operation.Parameters?.Add(new OpenApiParameter
        {
            Name = IORequestHeaderConstants.KeyID,
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = JsonSchemaType.String,
                Default = JsonValue.Create(keyID)
            }
        });
    }
}
