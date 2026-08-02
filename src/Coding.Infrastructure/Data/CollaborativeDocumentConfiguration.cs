using Coding.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coding.Data;

public sealed class CollaborativeDocumentSnapshotConfiguration : IEntityTypeConfiguration<CollaborativeDocumentSnapshot>
{
    public void Configure(EntityTypeBuilder<CollaborativeDocumentSnapshot> builder)
    {
        builder.HasKey(x => x.ID); builder.Property(x => x.EncodedState).IsRequired(); builder.Property(x => x.ContentHash).HasMaxLength(64).IsRequired();
        builder.HasIndex(x => new { x.FileId, x.SequenceNumber }).IsUnique();
        builder.HasOne<WorkspaceNode>().WithMany().HasForeignKey(x => x.FileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class CollaborativeDocumentUpdateConfiguration : IEntityTypeConfiguration<CollaborativeDocumentUpdate>
{
    public void Configure(EntityTypeBuilder<CollaborativeDocumentUpdate> builder)
    {
        builder.HasKey(x => x.ID); builder.Property(x => x.EncodedUpdate).IsRequired();
        builder.HasIndex(x => new { x.FileId, x.SequenceNumber }).IsUnique(); builder.HasIndex(x => new { x.FileId, x.UpdateId }).IsUnique();
        builder.HasOne<WorkspaceNode>().WithMany().HasForeignKey(x => x.FileId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Project>().WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
    }
}
