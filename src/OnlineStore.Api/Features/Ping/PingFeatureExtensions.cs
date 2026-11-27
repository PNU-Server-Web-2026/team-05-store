namespace OnlineStore.Api.Features.Ping;

public static class PingFeatureExtensions
{
    public static IServiceCollection AddPingFeature(this IServiceCollection services)
    {
        services.AddSingleton<IPingService, PingService>();
        return services;
    }
}
