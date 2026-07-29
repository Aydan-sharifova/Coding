using Coding.Infrastructure.Authentication;
using Coding.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Coding.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider services,
        bool seedDevelopmentData = false,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync(cancellationToken);

        var existingRoles = await context.Roles
            .Select(item => item.Name)
            .ToListAsync(cancellationToken);

        foreach (var roleName in SystemRoles.All.Except(existingRoles, StringComparer.OrdinalIgnoreCase))
        {
            context.Roles.Add(new Role
            {
                Name = roleName,
                Description = $"Built-in {roleName} role."
            });
        }

        await context.SaveChangesAsync(cancellationToken);

        if (seedDevelopmentData)
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DevelopmentDataSeeder>();
            await seeder.SeedAsync(cancellationToken);
        }
    }
}
