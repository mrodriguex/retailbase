using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

using HARD.CORE.API.Helpers;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add your custom DI configuration
builder.Services.AddApplicationServices(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();

// 1. Add API Versioning services (NEW WAY FOR .NET 6+)
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddMvc() // <- �Este .AddMvc() es NECESARIO ahora!
.AddApiExplorer(options => // <- Ahora esto funcionar�
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


// 2. Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // This is enough for basic setup

// 3. Configure the Swagger options USING THE CUSTOM CLASS WE WILL CREATE NEXT
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();


// Agrega la pol�tica de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigins",
        builder =>
        {
            //builder.WithOrigins("https://localhost:7209") // Dominio desde el que se permiten las solicitudes
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

//Config.Configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorPages();

// Configuración de JWT
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT key is missing in configuration (Jwt:Key).");
}
var key = Encoding.ASCII.GetBytes(jwtKey);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
    });

var app = builder.Build();

// Usar la política CORS
app.UseCors("AllowOrigins");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


// 5. Get the service that knows about all our API versions
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
// Habilitar el middleware de Swagger solo en el entorno de desarrollo
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    // 6. Build a Swagger endpoint for each discovered API version
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint(
            url: $"./{description.GroupName}/swagger.json",
            name: $"HARDCORE API {description.GroupName.ToLowerInvariant()}" // Display name in the dropdown
        );
    }

    // 7. (Optional) Set the default selection in the dropdown
    // options.RoutePrefix = string.Empty; // To serve Swagger UI at the root
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  // Importante: Colocar la autenticaci�n antes de la autorizaci�n
app.UseAuthorization();

app.MapControllers(); // Aseg�rate de que esta l�nea est� presente para manejar las rutas de los controladores.

app.MapRazorPages();

app.Run();
