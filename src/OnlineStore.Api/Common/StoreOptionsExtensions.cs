namespace OnlineStore.Api.Common;

public static class StoreOptionsExtensions
{
    public static IServiceCollection AddStoreOptions(this IServiceCollection services)
    {
        services
            .AddOptions<StoreOptions>()
            .BindConfiguration(StoreOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
