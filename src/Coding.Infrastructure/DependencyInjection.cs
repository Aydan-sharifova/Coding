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
using Coding.Application.Features.UserSettings;
using Coding.Infrastructure.UserSettings;
using Coding.Application.Features.AiAssistant;
using Coding.Infrastructure.AiAssistant;
using Coding.Infrastructure.Caching;
using Coding.Application.Abstractions;

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
        services.AddSingleton<ICacheService, MemoryCacheService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddOptions<SmtpOptions>().Bind(configuration.GetSection(SmtpOptions.SectionName))
            .Validate(x => !x.Enabled || (!string.IsNullOrWhiteSpace(x.Host) && x.Port is > 0 and <= 65535 && !string.IsNullOrWhiteSpace(x.FromEmail) && Uri.TryCreate(x.ClientBaseUrl, UriKind.Absolute, out _)), "Enabled SMTP requires a valid host, port, from address, and client base URL.")
            .ValidateOnStart();
        services.AddScoped<LoggingEmailSender>();
        services.AddScoped<SmtpEmailSender>();
        services.AddScoped<IEmailSender>(provider =>
            provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<SmtpOptions>>().Value.Enabled
                ? provider.GetRequiredService<SmtpEmailSender>()
                : provider.GetRequiredService<LoggingEmailSender>());
        services.AddScoped<IdentityPasswordService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IActivityLogger, ActivityLogger>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddOptions<OpenAiOptions>()
            .Bind(configuration.GetSection(OpenAiOptions.SectionName));
        services.AddHttpClient<OpenAiProvider>(client =>
            client.Timeout = Timeout.InfiniteTimeSpan);
        services.AddScoped<DevelopmentAiProvider>();
        services.AddScoped<IAiProvider>(provider =>
        {
            var options = provider
                .GetRequiredService<Microsoft.Extensions.Options.IOptions<OpenAiOptions>>()
                .Value;
            return options.IsConfigured
                ? provider.GetRequiredService<OpenAiProvider>()
                : provider.GetRequiredService<DevelopmentAiProvider>();
        });
        services.AddScoped<IAiContextBuilder, AiContextBuilder>();
        services.AddScoped<IAiPromptTemplateService, AiPromptTemplateService>();
        services.AddScoped<IAiUsageTracker, AiUsageTracker>();
        services.AddScoped<IAiConversationService, AiConversationService>();

        return services;
    }
}
