using Coding.Application.Features.Chat;
using Coding.Application.Features.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Coding.Api.Collaboration;

public sealed class ChatNotificationRealtimePublisher(
    IHubContext<CollaborationHub, ICollaborationClient> hub) :
    IChatRealtimePublisher,
    INotificationRealtimePublisher
{
    public async Task MessageReceivedAsync(ChatMessageItem message, IReadOnlyCollection<Guid> participantIds, CancellationToken ct)
    {
        await hub.Clients.Group(CollaborationHub.ConversationGroup(message.ConversationId)).ReceiveMessage(message);
        foreach (var userId in participantIds)
            await hub.Clients.Group(CollaborationHub.UserGroup(userId)).ConversationUpdated(message.ConversationId);
    }

    public Task ConversationReadAsync(Guid conversationId, Guid userId, Guid? throughMessageId, DateTime readAt, IReadOnlyCollection<Guid> participantIds, CancellationToken ct) =>
        hub.Clients.Group(CollaborationHub.ConversationGroup(conversationId)).MessageRead(conversationId, userId, throughMessageId, readAt);

    public async Task ConversationUpdatedAsync(Guid conversationId, IReadOnlyCollection<Guid> participantIds, CancellationToken ct)
    {
        foreach (var userId in participantIds)
            await hub.Clients.Group(CollaborationHub.UserGroup(userId)).ConversationUpdated(conversationId);
    }

    public Task NotificationReceivedAsync(NotificationItem notification, CancellationToken ct) =>
        hub.Clients.Group(CollaborationHub.UserGroup(notification.UserId)).ReceiveNotification(notification);

    public Task NotificationReadAsync(Guid userId, Guid? notificationId, CancellationToken ct) =>
        hub.Clients.Group(CollaborationHub.UserGroup(userId)).NotificationRead(notificationId);

    public Task UnreadCountUpdatedAsync(Guid userId, int count, CancellationToken ct) =>
        hub.Clients.Group(CollaborationHub.UserGroup(userId)).UnreadCountUpdated(count);
}
