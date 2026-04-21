using Asp.Viewsioning.ApiExplorer;

using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace RETAIL.BASE.API.Helpers // Replace with your project's namespace
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiViewsionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiViewsionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            // Create a Swagger document for each discovered API version
            foreach (var description in _provider.ApiViewsionDescriptions)
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

        private static OpenApiInfo CreateOpenApiInfo(ApiViewsionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "RETAIL_BASE API", // Your API's name
                Viewsion = description.ApiViewsion.ToString(),
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