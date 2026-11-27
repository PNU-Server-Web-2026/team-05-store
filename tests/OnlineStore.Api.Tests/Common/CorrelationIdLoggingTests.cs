using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using OnlineStore.Api.Common;

namespace OnlineStore.Api.Tests.Common;

public sealed class CorrelationIdLoggingTests
{
    [Fact]
    public async Task Get_JsonLogRecord_ContainsCorrelationIdFromScope()
    {
        const string incoming = "json-scope-correlation-id";
        var capture = new JsonScopeLogProvider();
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(capture);
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        await using var app = builder.Build();
        app.UseCorrelationId();
        app.MapGet("/probe", () => Results.Ok());
        await app.StartAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/probe");
        request.Headers.TryAddWithoutValidation(CorrelationIdMiddleware.HeaderName, incoming);

        var response = await app.GetTestClient().SendAsync(request);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(
            capture.Records,
            json => json.Contains(incoming, StringComparison.Ordinal)
                    && json.Contains(CorrelationIdMiddleware.ItemKey, StringComparison.Ordinal));
    }

    private sealed class JsonScopeLogProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly List<string> _records = [];
        private readonly object _lock = new();
        private IExternalScopeProvider? _scopeProvider;

        public IReadOnlyList<string> Records
        {
            get
            {
                lock (_lock)
                {
                    return [.. _records];
                }
            }
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider) => _scopeProvider = scopeProvider;

        public ILogger CreateLogger(string categoryName) => new CaptureLogger(categoryName, this);

        public void Dispose()
        {
        }

        private void Write(string categoryName, string message)
        {
            using var stream = new MemoryStream();
            using (var writer = new Utf8JsonWriter(stream))
            {
                writer.WriteStartObject();
                writer.WriteString("Category", categoryName);
                writer.WriteString("Message", message);
                writer.WritePropertyName("Scopes");
                writer.WriteStartArray();
                _scopeProvider?.ForEachScope(WriteScope, writer);
                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            var json = Encoding.UTF8.GetString(stream.ToArray());
            lock (_lock)
            {
                _records.Add(json);
            }
        }

        private static void WriteScope(object? scope, Utf8JsonWriter writer)
        {
            if (scope is IEnumerable<KeyValuePair<string, object>> pairs)
            {
                writer.WriteStartObject();
                foreach (var pair in pairs)
                {
                    writer.WriteString(pair.Key, Convert.ToString(pair.Value, CultureInfo.InvariantCulture));
                }

                writer.WriteEndObject();
                return;
            }

            writer.WriteStringValue(Convert.ToString(scope, CultureInfo.InvariantCulture));
        }

        private sealed class CaptureLogger(string categoryName, JsonScopeLogProvider provider) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

            public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }

                provider.Write(categoryName, formatter(state, exception));
            }
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
