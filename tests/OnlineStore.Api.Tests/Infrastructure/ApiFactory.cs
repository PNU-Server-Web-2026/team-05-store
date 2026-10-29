using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using OnlineStore.Api.Data;
using Respawn;
using Testcontainers.PostgreSql;

namespace OnlineStore.Api.Tests.Infrastructure;

/// <summary>
/// Starts one PostgreSQL container per test run, applies EF migrations, and resets data via Respawn.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string ConnectionStringSettingKey = "ConnectionStrings:Default";

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("online_store")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private NpgsqlConnection? _respawnConnection;
    private Respawner? _respawner;
    private bool _initialized;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        _ = CreateClient();

        await using (var scope = Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        _respawnConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await _respawnConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_respawnConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = ["__EFMigrationsHistory"]
        });

        _initialized = true;
    }

    public async Task ResetDatabaseAsync()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("ApiFactory is not initialized.");
        }

        await _respawner!.ResetAsync(_respawnConnection!);
    }

    public override async ValueTask DisposeAsync()
    {
        if (_respawnConnection is not null)
        {
            await _respawnConnection.DisposeAsync();
        }

        await _dbContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    Task IAsyncLifetime.DisposeAsync() => DisposeAsync().AsTask();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = _dbContainer.GetConnectionString();

        builder.UseEnvironment("Testing");
        builder.UseSetting(ConnectionStringSettingKey, connectionString);

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        });
    }
}
