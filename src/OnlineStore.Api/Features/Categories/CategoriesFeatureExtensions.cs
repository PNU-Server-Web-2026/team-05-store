namespace OnlineStore.Api.Features.Categories;

public static class CategoriesFeatureExtensions
{
    public static IServiceCollection AddCategoriesFeature(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        return services;
    }
}
