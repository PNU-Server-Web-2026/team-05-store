using OnlineStore.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Сервіси. Реєстрацію сервісів конкретних фіч додавайте через extension-методи
// у папці фічі (наприклад, builder.Services.AddHotelsFeature();), див. CONTRIBUTING.md.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Online Store / Warehouse API",
            Version = "v1",
            Description =
                "REST API інтернет-магазину з кількома складами: каталог товарів, облік залишків і життєвий цикл замовлень."
        };
        return Task.CompletedTask;
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// HTTP-конвеєр.
if (app.Environment.IsDevelopment())
{
    // OpenAPI-документ: /openapi/v1.json
    app.MapOpenApi();
    // Scalar UI: /scalar
    app.MapScalarApiReference();
}

app.MapControllers();

app.Run();

// Потрібно для інтеграційних тестів (WebApplicationFactory<Program>).
public partial class Program;
