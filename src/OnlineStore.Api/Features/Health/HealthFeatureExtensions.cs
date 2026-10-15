using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Api.Data;

namespace OnlineStore.Api.Features.Health;

/// <summary>
/// Методи розширення для реєстрації та мапінгу Health Checks (Liveness та Readiness).
/// </summary>
public static class HealthFeatureExtensions
{
    /// <summary>
    /// Реєструє сервіси перевірок стану (Health Checks), зокрема перевірку підключення до БД з тегом "ready".
    /// </summary>
    public static IServiceCollection AddHealthFeature(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>(
                name: "database",
                tags: ["ready"]);

        return services;
    }

    /// <summary>
    /// Мапить ендпоінти перевірок: /health (liveness) та /health/ready (readiness).
    /// </summary>
    public static WebApplication MapHealthFeature(this WebApplication app)
    {
        // Liveness: перевіряє лише, що процес живий (не виконує жодних перевірок).
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
        });

        // Readiness: перевіряє готовність до роботи (включає перевірку БД за тегом "ready").
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = HealthCheckResponseWriter.WriteJsonResponse
        });

        return app;
    }
}
