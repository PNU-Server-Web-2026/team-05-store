using Microsoft.EntityFrameworkCore;
using OnlineStore.Api.Data;

namespace OnlineStore.Api.Tests.Infrastructure;

public sealed class DatabaseResetTests(ApiFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task InsertProbeRow_TableContainsSingleRow()
    {
        var id = Guid.NewGuid();

        await ExecuteDbContextAsync(async db =>
        {
            db.IntegrationResetProbes.Add(new IntegrationResetProbe { Id = id, Label = "probe-a" });
            await db.SaveChangesAsync();
        });

        var count = await ExecuteDbContextAsync(db => db.IntegrationResetProbes.CountAsync());
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task AnotherInsertAfterReset_StillSingleRowNotAccumulated()
    {
        var id = Guid.NewGuid();

        await ExecuteDbContextAsync(async db =>
        {
            db.IntegrationResetProbes.Add(new IntegrationResetProbe { Id = id, Label = "probe-b" });
            await db.SaveChangesAsync();
        });

        var count = await ExecuteDbContextAsync(db => db.IntegrationResetProbes.CountAsync());
        Assert.Equal(1, count);
    }
}
