using Coding.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Coding.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Role> Roles => Set<Role>();

        public DbSet<UserRole> UserRoles => Set<UserRole>();

        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public DbSet<AccountToken> AccountTokens => Set<AccountToken>();

        public DbSet<Workspace> Workspaces => Set<Workspace>();

        public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();

        public DbSet<Project> Projects => Set<Project>();

        public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

        public DbSet<ProjectInvitation> ProjectInvitations => Set<ProjectInvitation>();

        public DbSet<WorkspaceNode> WorkspaceNodes => Set<WorkspaceNode>();
        public DbSet<FileContent> FileContents => Set<FileContent>();
        public DbSet<FileVersion> FileVersions => Set<FileVersion>();

        public DbSet<Folder> Folders => Set<Folder>();

        public DbSet<FileItem> FileItems => Set<FileItem>();

        public DbSet<CodeHistory> CodeHistories => Set<CodeHistory>();

        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<MessageReadReceipt> MessageReadReceipts => Set<MessageReadReceipt>();

        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<UserNotificationPreference> UserNotificationPreferences => Set<UserNotificationPreference>();

        public DbSet<Invitation> Invitations => Set<Invitation>();

        public DbSet<GitCommit> GitCommits => Set<GitCommit>();

        public DbSet<AIRequest> AIRequests => Set<AIRequest>();

        public DbSet<AIResponse> AIResponses => Set<AIResponse>();


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
