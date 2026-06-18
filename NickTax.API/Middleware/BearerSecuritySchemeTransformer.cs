using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace NickTax.API.Middleware;

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

        // Prossiga somente se a autenticação Bearer estiver configurada
        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            // Defina o esquema de segurança Bearer
            var bearerScheme = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Cabeçalho de autorização JWT usando o esquema Bearer."
            };

            // Garanta que os componentes estejam inicializados
            document.Components ??= new OpenApiComponents();

            // Adicione o esquema aos componentes do documento usando o método nativo do .NET 10
            document.AddComponent("Bearer", bearerScheme);

            // Crie um requisito de segurança referenciando o esquema de forma simplificada
            var securityRequirement = new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            };

            // Aplica o requisito a todas as operações
            var operations = document.Paths?.Values.SelectMany(p => p.Operations?.AsEnumerable() ?? []);

            if (operations != null)
            {
                foreach (var operation in operations)
                {
                    operation.Value.Security ??= new List<OpenApiSecurityRequirement>();
                    operation.Value.Security.Add(securityRequirement);
                }
            }
        }
    }
}