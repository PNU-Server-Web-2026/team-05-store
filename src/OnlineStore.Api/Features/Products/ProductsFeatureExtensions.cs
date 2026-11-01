namespace OnlineStore.Api.Features.Products;

public static class ProductsFeatureExtensions
{
    public static IServiceCollection AddProductsFeature(this IServiceCollection services)
    {
        // In-memory словник спільний для всіх запитів — тому Singleton.
        // Scoped/Transient дали б порожній склад на кожен HTTP-запит.
        services.AddSingleton<IProductService, InMemoryProductService>();
        return services;
    }
}
