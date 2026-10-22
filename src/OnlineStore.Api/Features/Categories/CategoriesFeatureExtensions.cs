namespace OnlineStore.Api.Features.Categories;

public static class CategoriesFeatureExtensions
{
    public static IServiceCollection AddCategoriesFeature(this IServiceCollection services)
    {
        // In-memory словник спільний для всіх запитів — тому Singleton.
        // Scoped/Transient дали б порожній склад на кожен HTTP-запит.
        services.AddSingleton<ICategoryService, InMemoryCategoryService>();
        return services;
    }
}
