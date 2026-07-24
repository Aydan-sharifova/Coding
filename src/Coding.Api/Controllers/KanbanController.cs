using Coding.Application.Features.Kanban;
using Coding.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coding.Controllers;

[ApiController, Authorize, Route("api")]
public sealed class KanbanController(ISender sender) : ControllerBase
{
    [HttpGet("projects/{projectId:guid}/tasks")] public Task<IReadOnlyList<ProjectTaskDto>> Board(Guid projectId, CancellationToken ct) => sender.Send(new GetProjectBoardQuery(projectId), ct);
    [HttpPost("projects/{projectId:guid}/tasks")] public async Task<ActionResult<ProjectTaskDto>> Create(Guid projectId, SaveTaskRequest request, CancellationToken ct) => StatusCode(201, await sender.Send(new CreateTaskCommand(projectId, request.Title, request.Description, request.Priority, request.DueDate), ct));
    [HttpPut("tasks/{taskId:guid}")] public Task<ProjectTaskDto> Update(Guid taskId, SaveTaskRequest request, CancellationToken ct) => sender.Send(new UpdateTaskCommand(taskId, request.Title, request.Description, request.Priority, request.DueDate), ct);
    [HttpDelete("tasks/{taskId:guid}")] public async Task<IActionResult> Delete(Guid taskId, CancellationToken ct) { await sender.Send(new DeleteTaskCommand(taskId), ct); return NoContent(); }
    [HttpPut("tasks/{taskId:guid}/position")] public Task<ProjectTaskDto> Move(Guid taskId, MoveTaskRequest request, CancellationToken ct) => sender.Send(new MoveTaskCommand(taskId, request.Status, request.PreviousTaskId, request.NextTaskId), ct);
    [HttpPost("tasks/{taskId:guid}/assignees/{userId:guid}")] public Task<ProjectTaskDto> Assign(Guid taskId, Guid userId, CancellationToken ct) => sender.Send(new AssignTaskMemberCommand(taskId, userId), ct);
    [HttpDelete("tasks/{taskId:guid}/assignees/{userId:guid}")] public Task<ProjectTaskDto> Unassign(Guid taskId, Guid userId, CancellationToken ct) => sender.Send(new RemoveTaskAssigneeCommand(taskId, userId), ct);
    [HttpPost("tasks/{taskId:guid}/comments")] public async Task<ActionResult<TaskCommentDto>> Comment(Guid taskId, AddTaskCommentRequest request, CancellationToken ct) => StatusCode(201, await sender.Send(new AddTaskCommentCommand(taskId, request.Content), ct));
    [HttpDelete("task-comments/{commentId:guid}")] public async Task<IActionResult> DeleteComment(Guid commentId, CancellationToken ct) { await sender.Send(new DeleteTaskCommentCommand(commentId), ct); return NoContent(); }
}
public sealed record SaveTaskRequest(string Title, string? Description, ProjectTaskPriority Priority, DateTime? DueDate);
public sealed record MoveTaskRequest(ProjectTaskStatus Status, Guid? PreviousTaskId, Guid? NextTaskId);
public sealed record AddTaskCommentRequest(string Content);
