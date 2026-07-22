using Coding.Application.Features.FileExplorer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers;

[ApiController, Authorize, Route("api")]
public sealed class FileExplorerController(ISender sender) : ControllerBase
{
    [HttpGet("projects/{projectId:guid}/nodes")]
    public Task<IReadOnlyList<WorkspaceNodeDto>> Tree(Guid projectId, CancellationToken ct) => sender.Send(new GetProjectFileTreeQuery(projectId), ct);
    [HttpGet("projects/{projectId:guid}/nodes/children")]
    public Task<IReadOnlyList<WorkspaceNodeDto>> Children(Guid projectId, [FromQuery] Guid? parentId, CancellationToken ct) => sender.Send(new GetFolderChildrenQuery(projectId, parentId), ct);
    [HttpPost("projects/{projectId:guid}/folders")]
    public async Task<ActionResult<WorkspaceNodeDto>> Folder(Guid projectId, CreateNodeRequest request, CancellationToken ct) => StatusCode(201, await sender.Send(new CreateFolderCommand(projectId, request.ParentId, request.Name), ct));
    [HttpPost("projects/{projectId:guid}/files")]
    public async Task<ActionResult<WorkspaceNodeDto>> File(Guid projectId, CreateFileRequest request, CancellationToken ct) => StatusCode(201, await sender.Send(new CreateFileCommand(projectId, request.ParentId, request.Name, request.Content ?? ""), ct));
    [HttpPut("nodes/{nodeId:guid}/name")]
    public Task<WorkspaceNodeDto> Rename(Guid nodeId, RenameNodeRequest request, CancellationToken ct) => sender.Send(new RenameNodeCommand(nodeId, request.Name), ct);
    [HttpPut("nodes/{nodeId:guid}/parent")]
    public Task<WorkspaceNodeDto> Move(Guid nodeId, MoveNodeRequest request, CancellationToken ct) => sender.Send(new MoveNodeCommand(nodeId, request.ParentId), ct);
    [HttpDelete("nodes/{nodeId:guid}")]
    public async Task<IActionResult> Delete(Guid nodeId, CancellationToken ct) { await sender.Send(new DeleteNodeCommand(nodeId), ct); return NoContent(); }
    [HttpPost("nodes/{nodeId:guid}/restore")]
    public async Task<IActionResult> Restore(Guid nodeId, CancellationToken ct) { await sender.Send(new RestoreDeletedNodeCommand(nodeId), ct); return NoContent(); }
    [HttpGet("files/{nodeId:guid}/content")]
    public Task<FileContentDto> Content(Guid nodeId, CancellationToken ct) => sender.Send(new GetFileContentQuery(nodeId), ct);
    [HttpPut("files/{nodeId:guid}/content")]
    public Task<FileContentDto> Save(Guid nodeId, SaveContentRequest request, CancellationToken ct) => sender.Send(new SaveFileContentCommand(nodeId, request.Content, request.ConcurrencyToken), ct);
    [HttpGet("files/{nodeId:guid}/versions")]
    public Task<IReadOnlyList<FileVersionDto>> Versions(Guid nodeId, CancellationToken ct) => sender.Send(new GetFileVersionsQuery(nodeId), ct);
    [HttpGet("files/{nodeId:guid}/versions/{versionId:guid}")]
    public Task<FileVersionDetails> Version(Guid nodeId, Guid versionId, CancellationToken ct) => sender.Send(new GetFileVersionByIdQuery(nodeId, versionId), ct);
    [HttpGet("files/{nodeId:guid}/versions/compare")]
    public Task<VersionComparison> Compare(Guid nodeId, [FromQuery] Guid leftId, [FromQuery] Guid rightId, CancellationToken ct) => sender.Send(new CompareFileVersionsQuery(nodeId, leftId, rightId), ct);
    [HttpPost("files/{nodeId:guid}/versions/{versionId:guid}/restore")]
    public Task<FileContentDto> RestoreVersion(Guid nodeId, Guid versionId, CancellationToken ct) => sender.Send(new RestoreFileVersionCommand(nodeId, versionId), ct);
}

public sealed record CreateNodeRequest(Guid? ParentId, string Name);
public sealed record CreateFileRequest(Guid? ParentId, string Name, string? Content);
public sealed record RenameNodeRequest(string Name);
public sealed record MoveNodeRequest(Guid? ParentId);
public sealed record SaveContentRequest(string Content, string ConcurrencyToken);
