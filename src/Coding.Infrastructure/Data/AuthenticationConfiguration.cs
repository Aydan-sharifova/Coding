using Coding.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coding.Data;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(item => item.Email).IsUnique();
        builder.HasIndex(item => item.UserName).IsUnique();
        builder.Property(item => item.Email).HasMaxLength(254);
        builder.Property(item => item.UserName).HasMaxLength(50);
        builder.Property(item => item.PasswordHash).HasMaxLength(100);
    }
}

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasIndex(item => item.Name).IsUnique();
        builder.Property(item => item.Name).HasMaxLength(50);
        builder.HasData(
            new Role
            {
                ID = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Admin",
                Description = "Built-in Admin role.",
                CreatAt = DateTime.UnixEpoch
            },
            new Role
            {
                ID = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Developer",
                Description = "Built-in Developer role.",
                CreatAt = DateTime.UnixEpoch
            },
            new Role
            {
                ID = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Guest",
                Description = "Built-in Guest role.",
                CreatAt = DateTime.UnixEpoch
            });
    }
}

public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasIndex(item => new { item.UserId, item.RoleId }).IsUnique();
    }
}

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasIndex(item => item.Token).IsUnique();
        builder.Property(item => item.Token).HasMaxLength(64);
        builder.HasOne(item => item.User)
            .WithMany(item => item.RefreshTokens)
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class AccountTokenConfiguration : IEntityTypeConfiguration<AccountToken>
{
    public void Configure(EntityTypeBuilder<AccountToken> builder)
    {
        builder.HasIndex(item => item.TokenHash).IsUnique();
        builder.Property(item => item.TokenHash).HasMaxLength(64);
        builder.HasOne(item => item.User)
            .WithMany(item => item.AccountTokens)
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
