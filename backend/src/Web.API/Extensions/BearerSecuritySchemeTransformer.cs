using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Web.Api.Extensions;

internal sealed class BearerSecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider
) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        // Only proceed if Bearer authentication is configured
        if (authenticationSchemes.All(authScheme => authScheme.Name != "Bearer"))
            return;
        var bearerScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme."
        };

        // Ensure components are initialized
        document.Components ??= new OpenApiComponents();

        // Add the scheme to the document components
        document.AddComponent("Bearer", bearerScheme);

        // Create a security requirement referencing the scheme
        var securityRequirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        };

        // Apply the requirement to all operations
#pragma warning disable S3267
        foreach (var operation in document.Paths.Values.SelectMany(p => p.Operations!))
#pragma warning restore S3267
        {
            operation.Value.Security ??= [];
            operation.Value.Security.Add(securityRequirement);
        }
        // Define the Bearer security scheme
    }
}