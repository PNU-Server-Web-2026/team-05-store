using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OnlineStore.Api.Common;
using OnlineStore.Api.Tests.Infrastructure;

namespace OnlineStore.Api.Tests.Common;

public sealed class StoreOptionsTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public void Options_AreBoundFromConfiguration()
    {
        var options = Factory.Services.GetRequiredService<IOptions<StoreOptions>>().Value;

        Assert.Equal(24, options.UnpaidOrderTimeoutHours);
        Assert.Equal(50, options.MaxItemsPerOrder);
    }

    [Fact]
    public void Options_CanBeOverriddenByConfiguration()
    {
        using var custom = Factory.WithWebHostBuilder(b =>
        {
            b.UseSetting("Store:UnpaidOrderTimeoutHours", "12");
            b.UseSetting("Store:MaxItemsPerOrder", "10");
        });

        var options = custom.Services.GetRequiredService<IOptions<StoreOptions>>().Value;

        Assert.Equal(12, options.UnpaidOrderTimeoutHours);
        Assert.Equal(10, options.MaxItemsPerOrder);
    }

    [Fact]
    public void InvalidMaxItemsPerOrder_FailsOnStartupWithFieldName()
    {
        using var broken = Factory.WithWebHostBuilder(b => b.UseSetting("Store:MaxItemsPerOrder", "0"));

        var exception = Assert.Throws<OptionsValidationException>(() => broken.CreateClient());
        Assert.Contains(nameof(StoreOptions.MaxItemsPerOrder), exception.Message);
    }

    [Fact]
    public void InvalidUnpaidOrderTimeoutHours_FailsOnStartupWithFieldName()
    {
        using var broken = Factory.WithWebHostBuilder(b => b.UseSetting("Store:UnpaidOrderTimeoutHours", "0"));

        var exception = Assert.Throws<OptionsValidationException>(() => broken.CreateClient());
        Assert.Contains(nameof(StoreOptions.UnpaidOrderTimeoutHours), exception.Message);
    }
}
