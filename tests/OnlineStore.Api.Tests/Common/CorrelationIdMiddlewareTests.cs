using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OnlineStore.Api.Common;

namespace OnlineStore.Api.Tests.Common;

public sealed class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task Get_WhenHeaderMissing_GeneratesGuidAndReturnsIt()
    {
        await using var host = await StartProbeAsync();

        var response = await host.Client.GetAsync("/probe");
        var header = GetHeader(response);
        var body = await response.Content.ReadAsStringAsync();

        Assert.True(Guid.TryParse(header, out _));
        Assert.Equal(header, body);
        Assert.Equal(header, host.Logger.CorrelationId);
    }

    [Fact]
    public async Task Get_WhenHeaderPresent_EchoesSameValue()
    {
        const string incoming = "client-correlation-id";
        await using var host = await StartProbeAsync();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/probe");
        request.Headers.TryAddWithoutValidation(CorrelationIdMiddleware.HeaderName, incoming);

        var response = await host.Client.SendAsync(request);

        Assert.Equal(incoming, GetHeader(response));
        Assert.Equal(incoming, await response.Content.ReadAsStringAsync());
        Assert.Equal(incoming, host.Logger.CorrelationId);
    }

    [Fact]
    public async Task Get_WhenHeaderIsWhitespace_GeneratesGuid()
    {
        await using var host = await StartProbeAsync();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/probe");
        request.Headers.TryAddWithoutValidation(CorrelationIdMiddleware.HeaderName, "   ");

        var response = await host.Client.SendAsync(request);
        var header = GetHeader(response);

        Assert.True(Guid.TryParse(header, out _));
        Assert.NotEqual("   ", header);
    }

    private static async Task<ProbeHost> StartProbeAsync()
    {
        var logger = new ScopeCaptureLogger();
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Logging.ClearProviders();
        builder.Services.AddSingleton<ILogger<CorrelationIdMiddleware>>(logger);

        var app = builder.Build();
        app.UseCorrelationId();
        app.MapGet("/probe", (HttpContext context) =>
        {
            var correlationId = context.Items[CorrelationIdMiddleware.ItemKey]?.ToString();
            return Results.Text(correlationId ?? string.Empty);
        });

        await app.StartAsync();
        return new ProbeHost(app, app.GetTestClient(), logger);
    }

    private static string GetHeader(HttpResponseMessage response)
    {
        Assert.True(response.Headers.TryGetValues(CorrelationIdMiddleware.HeaderName, out var values));
        return Assert.Single(values);
    }

    private sealed class ProbeHost(WebApplication app, HttpClient client, ScopeCaptureLogger logger)
        : IAsyncDisposable
    {
        public HttpClient Client { get; } = client;

        public ScopeCaptureLogger Logger { get; } = logger;

        public async ValueTask DisposeAsync()
        {
            Client.Dispose();
            await app.DisposeAsync();
        }
    }

    private sealed class ScopeCaptureLogger : ILogger<CorrelationIdMiddleware>
    {
        public string? CorrelationId { get; private set; }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            if (state is IReadOnlyDictionary<string, object> values
                && values.TryGetValue(CorrelationIdMiddleware.ItemKey, out var value))
            {
                CorrelationId = value.ToString();
            }

            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}
