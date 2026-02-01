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
        IOCacheObject? authorizationCache = IOCache.GetCachedObject(IOCacheKeys.SwaggerAuthorization);
        string authorization = "";
        if (authorizationCache != null)
        {
            authorization = (string)authorizationCache.Value;
        }

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = IORequestHeaderConstants.Authorization,
            In = ParameterLocation.Header,
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString(authorization)
            }
        });

        string keyID = "";
        IOCacheObject? keyIDCacheObject = IOCache.GetCachedObject(IOCacheKeys.RSAPrivateKeyIDCacheKey);
        if (keyIDCacheObject != null) 
        {
            keyID = (string)keyIDCacheObject.Value;
        }
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = IORequestHeaderConstants.KeyID,
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString(keyID)
            }
        });

        string sessionID = "";
        IOCacheObject? sessionIDObject = IOCache.GetCachedObject(IOCacheKeys.SwaggerSessionID);
        if (sessionIDObject != null) 
        {
            sessionID = (string)sessionIDObject.Value;
        }
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = IORequestHeaderConstants.SessionID,
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString(sessionID)
            }
        });

        string symmetricKey = "";
        IOCacheObject? symmetricKeyObject = IOCache.GetCachedObject(IOCacheKeys.SwaggerSymmetricKey);
        if (symmetricKeyObject != null) 
        {
            symmetricKey = (string)symmetricKeyObject.Value;
        }
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = IORequestHeaderConstants.SymmetricKey,
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString(symmetricKey)
            }
        });

        string symmetricIV = "";
        IOCacheObject? symmetricIVObject = IOCache.GetCachedObject(IOCacheKeys.SwaggerSymmetricIV);
        if (symmetricIVObject != null) 
        {
            symmetricIV = (string)symmetricIVObject.Value;
        }
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = IORequestHeaderConstants.SymmetricIV,
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString(symmetricIV)
            }
        });

        string token = "";
        IOCacheObject? tokenObject = IOCache.GetCachedObject(IOCacheKeys.SwaggerToken);
        if (tokenObject != null) 
        {
            token = (string)tokenObject.Value;
        }
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = IORequestHeaderConstants.AuthorizationToken,
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString(token)
            }
        });

        string tokenExtra = "";
        IOCacheObject? tokenExtraObject = IOCache.GetCachedObject(IOCacheKeys.SwaggerTokenExtra);
        if (tokenExtraObject != null) 
        {
            tokenExtra = (string)tokenExtraObject.Value;
        }
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = IORequestHeaderConstants.AuthorizationTokenExtras,
            In = ParameterLocation.Header,
            Required = false,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Default = new OpenApiString(tokenExtra)
            }
        });
    }
}
