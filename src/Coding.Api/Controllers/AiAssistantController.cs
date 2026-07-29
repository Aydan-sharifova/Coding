using System.Text.Json;
using Coding.Application.Features.AiAssistant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Coding.Controllers;

[ApiController, Authorize, Route("api/ai"), EnableRateLimiting("ai")]
public sealed class AiAssistantController(
    IAiConversationService conversations,
    ILogger<AiAssistantController> logger) : ControllerBase
{
    [HttpGet("projects/{projectId:guid}/conversations")]
    public Task<IReadOnlyList<AiConversationDto>> GetConversations(Guid projectId, CancellationToken cancellationToken) =>
        conversations.GetConversationsAsync(projectId, cancellationToken);

    [HttpGet("conversations/{conversationId:guid}")]
    public Task<AiConversationDetails> GetConversation(Guid conversationId, CancellationToken cancellationToken) =>
        conversations.GetConversationAsync(conversationId, cancellationToken);

    [HttpPost("stream")]
    public async Task Stream(AiAssistantRequest request, CancellationToken cancellationToken)
    {
        Response.StatusCode = StatusCodes.Status200OK;
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache, no-transform";
        Response.Headers.Append("X-Accel-Buffering", "no");

        try
        {
            await foreach (var chunk in conversations.StreamAsync(request, cancellationToken).WithCancellation(cancellationToken))
                await WriteChunkAsync(chunk, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
                "AI generation was cancelled for project {ProjectId}.",
                request.ProjectId);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "AI generation failed for project {ProjectId} and action {Action}.",
                request.ProjectId,
                request.Action);

            if (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                var message = exception is InvalidOperationException or ArgumentException
                    ? exception.Message
                    : "AI generation failed. Check the API configuration and try again.";
                await WriteChunkAsync(
                    new AiStreamChunk(
                        string.Empty,
                        IsCompleted: true,
                        FinishReason: "error",
                        Error: message),
                    CancellationToken.None);
            }
        }
    }

    private async Task WriteChunkAsync(AiStreamChunk chunk, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(
            chunk,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
        await Response.Body.FlushAsync(cancellationToken);
    }
}
