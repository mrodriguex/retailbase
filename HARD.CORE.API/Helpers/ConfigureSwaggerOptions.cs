using Asp.Versioning.ApiExplorer;

using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace HARD.CORE.API.Helpers // Replace with your project's namespace
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            // Create a Swagger document for each discovered API version
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName, // e.g., "v1", "v2.0", "v3.0-beta"
                    CreateOpenApiInfo(description));
                // Definición del esquema de seguridad para JWT               
            }
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your valid JWT token.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

        }

        private static OpenApiInfo CreateOpenApiInfo(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "HARDCORE API", // Your API's name
                Version = description.ApiVersion.ToString(),
                Description = "API del Código Base HARD CORE.",
                Contact = new OpenApiContact { Name = "Manuel Rodríguez Camacho", Email = "mrodriguez@cryoinfra.com.mx" },
                License = new OpenApiLicense { Name = "MIT License" }
            };

            // Add a warning for deprecated versions
            if (description.IsDeprecated)
            {
                info.Description += " <strong>This API version has been deprecated. Please use a newer version.</strong>";
            }

            return info;
        }
    }
}