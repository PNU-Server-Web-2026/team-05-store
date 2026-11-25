namespace OnlineStore.Api.Features.Products;

public static class ProductsFeatureExtensions
{
    public static IServiceCollection AddProductsFeature(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        return services;
    }
}
