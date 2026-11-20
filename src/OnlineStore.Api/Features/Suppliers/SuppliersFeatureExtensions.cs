namespace OnlineStore.Api.Features.Suppliers;

public static class SuppliersFeatureExtensions
{
    public static IServiceCollection AddSuppliersFeature(this IServiceCollection services)
    {
        // In-memory словник спільний для всіх запитів — тому Singleton.
        // Scoped/Transient дали б порожній склад на кожен HTTP-запит.
        services.AddSingleton<ISupplierService, InMemorySupplierService>();
        return services;
    }
}
