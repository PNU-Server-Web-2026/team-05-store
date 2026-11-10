using System.Net;
using System.Text.Json;

namespace OnlineStore.Api.Tests.Common;

internal static class ValidationProblemAssert
{
    public static async Task AssertHasFieldErrorAsync(HttpResponseMessage response, string fieldName)
    {
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var mediaType = response.Content.Headers.ContentType?.MediaType;
        Assert.True(
            mediaType is "application/problem+json" or "application/json",
            $"Unexpected content type: {mediaType}");

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);

        var root = document.RootElement;
        Assert.Equal(400, root.GetProperty("status").GetInt32());
        Assert.True(root.TryGetProperty("errors", out var errors));

        var camelName = char.ToLowerInvariant(fieldName[0]) + fieldName[1..];
        var hasError = errors.TryGetProperty(fieldName, out var fieldErrors)
            || errors.TryGetProperty(camelName, out fieldErrors);
        Assert.True(hasError, $"Expected validation error for '{fieldName}' or '{camelName}'. Errors: {errors}");
        Assert.True(fieldErrors.GetArrayLength() > 0);
    }
}
