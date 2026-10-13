using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OnlineStore.Api.Common;

namespace OnlineStore.Api.Tests.Common;

public class StoreOptionsTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    [Fact]
    public void Options_AreBoundFromConfiguration()
    {
        var options = factory.Services.GetRequiredService<IOptions<StoreOptions>>().Value;

        Assert.Equal(24, options.UnpaidOrderTimeoutHours);
        Assert.Equal(50, options.MaxItemsPerOrder);
    }

    [Fact]
    public void Options_CanBeOverriddenByConfiguration()
    {
        using var custom = factory.WithWebHostBuilder(b =>
        {
            b.UseSetting("Store:UnpaidOrderTimeoutHours", "12");
            b.UseSetting("Store:MaxItemsPerOrder", "10");
        });

        var options = custom.Services.GetRequiredService<IOptions<StoreOptions>>().Value;

        Assert.Equal(12, options.UnpaidOrderTimeoutHours);
        Assert.Equal(10, options.MaxItemsPerOrder);
    }

    [Fact]
    public void InvalidOptions_FailOnStartup()
    {
        using var broken = factory.WithWebHostBuilder(b => b.UseSetting("Store:MaxItemsPerOrder", "0"));

        Assert.Throws<OptionsValidationException>(() => broken.CreateClient());
    }
}
