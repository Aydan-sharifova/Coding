 using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Coding.Api.Configuration;

public sealed class SwaggerAuthorizationOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        if (metadata.OfType<IAllowAnonymous>().Any())
        {
            operation.Security = [];
            return;
        }

        var authorizeData = metadata.OfType<IAuthorizeData>().ToArray();
        if (authorizeData.Length == 0)
        {
            operation.Security = [];
            return;
        }

        operation.Security =
        [
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }] = Array.Empty<string>()
            }
        ];

        operation.Responses.TryAdd("401", new OpenApiResponse
        {
            Description = "Authentication is required. Use Swagger's Authorize button with a valid access token."
        });

        var roles = authorizeData
            .SelectMany(data => (data.Roles ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (roles.Length > 0)
        {
            operation.Responses.TryAdd("403", new OpenApiResponse
            {
                Description = $"The authenticated user must have one of these roles: {string.Join(", ", roles)}."
            });
            operation.Description = string.IsNullOrWhiteSpace(operation.Description)
                ? $"Required role: {string.Join(" or ", roles)}."
                : $"{operation.Description}\n\nRequired role: {string.Join(" or ", roles)}.";
        }
    }
}
