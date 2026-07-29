using Coding.Infrastructure.Authentication;
using Coding.Application.Features.Demo;
using Coding.Infrastructure.Demo;
using Coding.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Coding.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider services,
        bool seedDevelopmentData = false,
        bool seedDemoData = false,
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

        if (seedDemoData)
        {
            var demoEnvironment =
                scope.ServiceProvider.GetRequiredService<IDemoEnvironmentService>();
            demoEnvironment.EnsureAvailable();
            var seeder = scope.ServiceProvider.GetRequiredService<DemoDataSeeder>();
            await seeder.SeedAsync(cancellationToken);
        }
    }

    public static async Task ResetDemoEnvironmentAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var demoEnvironment =
            scope.ServiceProvider.GetRequiredService<IDemoEnvironmentService>();
        await demoEnvironment.ResetAsync(cancellationToken);
    }
}
