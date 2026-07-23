using System.Collections.Concurrent;

namespace Coding.Api.Collaboration;

public sealed record ConnectionPresence(
    string ConnectionId,
    Guid UserId,
    string UserName,
    string DisplayName,
    string? AvatarUrl,
    DateTime LastHeartbeat,
    HashSet<Guid> ProjectIds,
    HashSet<Guid> FileIds);

public interface ICollaborationPresenceTracker
{
    ConnectionPresence Connect(string connectionId, Guid userId, string userName, string displayName, string? avatarUrl);
    ConnectionPresence? Disconnect(string connectionId);
    bool JoinProject(string connectionId, Guid projectId);
    bool LeaveProject(string connectionId, Guid projectId);
    bool JoinFile(string connectionId, Guid fileId);
    bool LeaveFile(string connectionId, Guid fileId);
    void Heartbeat(string connectionId);
    bool IsInProject(string connectionId, Guid projectId);
    bool IsInFile(string connectionId, Guid fileId);
    IReadOnlyCollection<CollaborationUser> GetProjectUsers(Guid projectId);
    IReadOnlyCollection<ConnectionPresence> RemoveStale(DateTime cutoff);
    int GetProjectConnectionCount(Guid projectId, Guid userId);
}

public sealed class CollaborationPresenceTracker : ICollaborationPresenceTracker
{
    private readonly ConcurrentDictionary<string, ConnectionPresence> _connections = new();

    public ConnectionPresence Connect(string connectionId, Guid userId, string userName, string displayName, string? avatarUrl)
    {
        var connection = new ConnectionPresence(
            connectionId, userId, userName, displayName, avatarUrl,
            DateTime.UtcNow, [], []);
        _connections[connectionId] = connection;
        return connection;
    }

    public ConnectionPresence? Disconnect(string connectionId) =>
        _connections.TryRemove(connectionId, out var connection) ? connection : null;

    public bool JoinProject(string connectionId, Guid projectId) =>
        Mutate(connectionId, connection => connection.ProjectIds.Add(projectId));

    public bool LeaveProject(string connectionId, Guid projectId) =>
        Mutate(connectionId, connection =>
        {
            connection.FileIds.Clear();
            return connection.ProjectIds.Remove(projectId);
        });

    public bool JoinFile(string connectionId, Guid fileId) =>
        Mutate(connectionId, connection => connection.FileIds.Add(fileId));

    public bool LeaveFile(string connectionId, Guid fileId) =>
        Mutate(connectionId, connection => connection.FileIds.Remove(fileId));

    public void Heartbeat(string connectionId)
    {
        if (_connections.TryGetValue(connectionId, out var connection))
            _connections[connectionId] = connection with { LastHeartbeat = DateTime.UtcNow };
    }

    public bool IsInProject(string connectionId, Guid projectId) =>
        _connections.TryGetValue(connectionId, out var connection) &&
        connection.ProjectIds.Contains(projectId);

    public bool IsInFile(string connectionId, Guid fileId) =>
        _connections.TryGetValue(connectionId, out var connection) &&
        connection.FileIds.Contains(fileId);

    public IReadOnlyCollection<CollaborationUser> GetProjectUsers(Guid projectId) =>
        _connections.Values
            .Where(connection => connection.ProjectIds.Contains(projectId))
            .GroupBy(connection => connection.UserId)
            .Select(group =>
            {
                var latest = group.MaxBy(connection => connection.LastHeartbeat)!;
                return new CollaborationUser(
                    group.Key,
                    latest.UserName,
                    latest.DisplayName,
                    latest.AvatarUrl,
                    group.Count(),
                    latest.LastHeartbeat);
            })
            .OrderBy(user => user.DisplayName)
            .ToArray();

    public IReadOnlyCollection<ConnectionPresence> RemoveStale(DateTime cutoff)
    {
        var removed = new List<ConnectionPresence>();
        foreach (var item in _connections.Where(item => item.Value.LastHeartbeat < cutoff))
            if (_connections.TryRemove(item.Key, out var connection))
                removed.Add(connection);
        return removed;
    }

    public int GetProjectConnectionCount(Guid projectId, Guid userId) =>
        _connections.Values.Count(connection =>
            connection.UserId == userId && connection.ProjectIds.Contains(projectId));

    private bool Mutate(string connectionId, Func<ConnectionPresence, bool> action)
    {
        if (!_connections.TryGetValue(connectionId, out var connection))
            return false;
        lock (connection)
            return action(connection);
    }
}
