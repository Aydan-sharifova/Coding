using Coding.Data;
using Coding.Services.Implaments;
using Coding.Services.Interfaces;
using Coding.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Coding.Application.Features.Notifications;
using Coding.Infrastructure.Notifications;
using Coding.Application.Features.Activities;
using Coding.Infrastructure.Activities;
using Coding.Application.Features.AiAssistant;
using Coding.Infrastructure.AiAssistant;

namespace Coding.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "The PostgreSQL connection string 'Default' is not configured.");
        }

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                npgsql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }));

        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("postgresql");

        services.AddScoped(typeof(ICrudService<,,,>), typeof(CrudService<,,,>));
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IEmailSender, LoggingEmailSender>();
        services.AddScoped<IdentityPasswordService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IActivityLogger, ActivityLogger>();
        services.AddOptions<AiOptions>().Bind(configuration.GetSection(AiOptions.SectionName)).Validate(x => !string.IsNullOrWhiteSpace(x.Provider), "An AI provider name is required.").ValidateOnStart();
        services.AddScoped<IAiContextBuilder, AiContextBuilder>();
        services.AddScoped<IAiPromptTemplateService, AiPromptTemplateService>();
        services.AddScoped<IAiUsageTracker, AiUsageTracker>();
        services.AddScoped<IAiConversationService, AiConversationService>();
        services.AddScoped<DevelopmentAiProvider>();
        services.AddScoped<OpenAiProvider>();
        services.AddScoped<IAiProvider>(provider =>
        {
            var settings = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AiOptions>>().Value;
            return settings.Provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase)
                ? provider.GetRequiredService<OpenAiProvider>()
                : provider.GetRequiredService<DevelopmentAiProvider>();
        });

        return services;
    }
}
