using Microsoft.Extensions.DependencyInjection;

namespace OnlineStore.Api.Features.Warehouses;

/// <summary>
/// Методи розширення для реєстрації залежностей фічі складів.
/// </summary>
public static class WarehousesFeatureExtensions
{
    /// <summary>
    /// Реєструє сервіс керування складами в DI-контейнері.
    /// Використовується Singleton, оскільки стан зберігається в пам'яті (ConcurrentDictionary).
    /// При використанні Scoped або Transient стан скидався б на кожному HTTP-запиті.
    /// </summary>
    public static IServiceCollection AddWarehousesFeature(this IServiceCollection services)
    {
        services.AddSingleton<IWarehouseService, InMemoryWarehouseService>();
        return services;
    }
}
