using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace OnlineStore.Api.Features.Health;

/// <summary>
/// Форматує відповідь health check у структурований JSON.
/// </summary>
public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Записує стан перевірок здоров'я у форматі JSON у відповідь HTTP.
    /// Текст винятку виводиться лише в середовищі Development, щоб не розкривати деталі інфраструктури.
    /// </summary>
    public static Task WriteJsonResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var hostEnvironment = context.RequestServices.GetService<IHostEnvironment>();
        var isDevelopment = hostEnvironment?.IsDevelopment() ?? false;

        var payload = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.ToString(),
            entries = report.Entries.ToDictionary(
                entry => entry.Key,
                entry => new
                {
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    duration = entry.Value.Duration.ToString(),
                    error = isDevelopment ? entry.Value.Exception?.Message : null,
                    tags = entry.Value.Tags
                })
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(payload, SerializerOptions));
    }
}
