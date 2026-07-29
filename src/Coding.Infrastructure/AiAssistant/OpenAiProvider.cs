using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Coding.Application.Features.AiAssistant;
using Coding.Enums;
using Microsoft.Extensions.Options;

namespace Coding.Infrastructure.AiAssistant;

public sealed class OpenAiOptions
{
    public const string SectionName = "OpenAI";

    public string ApiKey { get; init; } = string.Empty;
    public string Model { get; init; } = "gpt-5.6-sol";
    public string BaseUrl { get; init; } = "https://api.openai.com/v1/";
    public int MaxOutputTokens { get; init; } = 4_096;

    public bool IsConfigured => !string.IsNullOrWhiteSpace(ApiKey);
}

public sealed class OpenAiProvider(
    HttpClient httpClient,
    IOptions<OpenAiOptions> options) : IAiProvider
{
    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web);

    private readonly OpenAiOptions _options = options.Value;

    public string ProviderName => "OpenAI";
    public string Model => _options.Model;

    public async IAsyncEnumerable<AiStreamChunk> StreamAsync(
        AiRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (!_options.IsConfigured)
            throw new InvalidOperationException(
                "OpenAI is not configured. Set the OpenAI__ApiKey environment variable.");

        using var message = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(new Uri(EnsureTrailingSlash(_options.BaseUrl)), "responses"));
        message.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        message.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("text/event-stream"));
        message.Content = JsonContent.Create(
            BuildPayload(request),
            options: JsonOptions);

        using var response = await httpClient.SendAsync(
            message,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"OpenAI returned {(int)response.StatusCode}: {ReadErrorMessage(body)}");
        }

        await using var stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);
        var completed = false;

        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line is null || !line.StartsWith("data:", StringComparison.Ordinal))
                continue;

            var data = line[5..].Trim();
            if (data.Length == 0 || data == "[DONE]")
                continue;

            using var document = JsonDocument.Parse(data);
            var root = document.RootElement;
            var eventType = root.TryGetProperty("type", out var type)
                ? type.GetString()
                : null;

            if (eventType == "response.output_text.delta" &&
                root.TryGetProperty("delta", out var delta))
            {
                var text = delta.GetString();
                if (!string.IsNullOrEmpty(text))
                    yield return new AiStreamChunk(text);
                continue;
            }

            if (eventType == "response.completed")
            {
                completed = true;
                var responseNode = root.GetProperty("response");
                var usage = responseNode.TryGetProperty("usage", out var value)
                    ? value
                    : default;
                yield return new AiStreamChunk(
                    string.Empty,
                    IsCompleted: true,
                    InputTokens: ReadInt(usage, "input_tokens"),
                    OutputTokens: ReadInt(usage, "output_tokens"),
                    FinishReason: "stop");
                continue;
            }

            if (eventType is "error" or "response.failed")
                throw new InvalidOperationException(ReadStreamingError(root));
        }

        if (!completed)
            yield return new AiStreamChunk(
                string.Empty,
                IsCompleted: true,
                FinishReason: "stream_ended");
    }

    private object BuildPayload(AiRequest request)
    {
        var input = request.History
            .Select(message => new
            {
                role = message.Role == AiMessageRole.Assistant
                    ? "assistant"
                    : "user",
                content = message.Content
            })
            .Cast<object>()
            .ToList();

        var userInput = new StringBuilder()
            .AppendLine($"Requested action: {request.Action}")
            .AppendLine($"Programming language: {request.ProgrammingLanguage}")
            .AppendLine()
            .AppendLine(request.UserInstructions);

        if (!string.IsNullOrWhiteSpace(request.RepositoryContext))
        {
            userInput
                .AppendLine()
                .AppendLine("Repository reference material follows:")
                .AppendLine(request.RepositoryContext);
        }

        var images = request.Images ?? [];
        if (images.Count == 0)
        {
            input.Add(new { role = "user", content = userInput.ToString() });
        }
        else
        {
            var parts = new List<object>
            {
                new { type = "input_text", text = userInput.ToString() }
            };
            parts.AddRange(images.Select(image => new
            {
                type = "input_image",
                image_url = $"data:{image.MediaType};base64,{image.Base64Content}"
            }));
            input.Add(new { role = "user", content = parts });
        }

        return new
        {
            model = _options.Model,
            instructions = request.SystemInstructions,
            input,
            stream = true,
            store = false,
            max_output_tokens = Math.Clamp(_options.MaxOutputTokens, 256, 32_768)
        };
    }

    private static int? ReadInt(JsonElement element, string propertyName) =>
        element.ValueKind == JsonValueKind.Object &&
        element.TryGetProperty(propertyName, out var value) &&
        value.TryGetInt32(out var result)
            ? result
            : null;

    private static string ReadStreamingError(JsonElement root)
    {
        if (root.TryGetProperty("error", out var error) &&
            error.ValueKind == JsonValueKind.Object &&
            error.TryGetProperty("message", out var message))
            return message.GetString() ?? "The OpenAI stream failed.";

        return "The OpenAI stream failed.";
    }

    private static string ReadErrorMessage(string body)
    {
        try
        {
            using var document = JsonDocument.Parse(body);
            if (document.RootElement.TryGetProperty("error", out var error) &&
                error.TryGetProperty("message", out var message))
                return message.GetString() ?? "Request failed.";
        }
        catch (JsonException)
        {
            // Return a bounded plain-text error when the upstream response is not JSON.
        }

        return body.Length <= 500 ? body : body[..500];
    }

    private static string EnsureTrailingSlash(string value) =>
        value.EndsWith("/", StringComparison.Ordinal) ? value : value + "/";
}
