using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace IOBootstrap.NET.Application.Filters;

public class IODefaultHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (string.Equals(context.ApiDescription.HttpMethod, HttpMethod.Post.Method, StringComparison.InvariantCultureIgnoreCase))
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-IO-AUTHORIZATION",
                In = ParameterLocation.Header,
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString("AUTHORIZATION")
                }
            });
        }
    }
}
