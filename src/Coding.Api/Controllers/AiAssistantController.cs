using System.Text.Json;
using System.Text.Json.Serialization;
using Coding.Application.Features.AiAssistant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers;

[ApiController, Authorize, Route("api/ai")]
public sealed class AiAssistantController(IAiConversationService conversations, ILogger<AiAssistantController> logger) : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    [HttpGet("projects/{projectId:guid}/conversations")]
    public Task<IReadOnlyList<AiConversationDto>> List(Guid projectId, CancellationToken ct) => conversations.GetConversationsAsync(projectId, ct);

    [HttpGet("conversations/{conversationId:guid}")]
    public Task<AiConversationDetails> Get(Guid conversationId, CancellationToken ct) => conversations.GetConversationAsync(conversationId, ct);

    [HttpPost("stream")]
    public async Task Stream(AiAssistantRequest request, CancellationToken ct)
    {
        Response.StatusCode = StatusCodes.Status200OK;
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache, no-transform";
        Response.Headers.Connection = "keep-alive";
        Response.Headers["X-Accel-Buffering"] = "no";
        try
        {
            await foreach (var chunk in conversations.StreamAsync(request, ct))
            {
                await Response.WriteAsync($"event: chunk\ndata: {JsonSerializer.Serialize(chunk, JsonOptions)}\n\n", ct);
                await Response.Body.FlushAsync(ct);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            logger.LogInformation("AI generation was cancelled by the client.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "AI generation failed.");
            if (!Response.HasStarted) throw;
            var error = JsonSerializer.Serialize(new { message = "AI generation failed." }, JsonOptions);
            await Response.WriteAsync($"event: error\ndata: {error}\n\n", CancellationToken.None);
            await Response.Body.FlushAsync(CancellationToken.None);
        }
    }
}
