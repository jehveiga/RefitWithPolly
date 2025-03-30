using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using WebApi.Extensions;
using WebApi.ExternalApis.WebApiFail;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddSerilog();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddRefitClientWithResilience<IWebApiFail>("https://localhost:7256");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Refit Web API",
        Description = "An ASP.NET Core Web API for example from Refit resilience",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    // Adiciona suporte ao Refit para que ele gere corretamente os contratos no Swagger
    options.SupportNonNullableReferenceTypes();
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(static options =>
    {
        options.OpenApiVersion = OpenApiSpecVersion.OpenApi2_0;
    });

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();