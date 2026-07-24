using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Coding.Api.Infrastructure;
using System.Threading.RateLimiting;
using Coding.Application.Abstractions;
using Coding.Application.Behaviors;
using Coding.Application.Features.Projects;
using Coding.Infrastructure.Projects;
using FluentValidation;
using MediatR;
using System.Text.Json.Serialization;
using Coding.Api.Collaboration;
using Coding.Application.Features.Chat;
using Coding.Application.Features.Notifications;

namespace Coding.Api.Configuration;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddEndpointsApiExplorer();
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddHttpClient();
        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        services.AddSingleton<ICollaborationPresenceTracker, CollaborationPresenceTracker>();
        services.AddHostedService<StaleConnectionCleanupService>();
        services.AddSingleton<ChatNotificationRealtimePublisher>();
        services.AddSingleton<IChatRealtimePublisher>(provider => provider.GetRequiredService<ChatNotificationRealtimePublisher>());
        services.AddSingleton<INotificationRealtimePublisher>(provider => provider.GetRequiredService<ChatNotificationRealtimePublisher>());
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(
            typeof(CreateProjectCommand).Assembly,
            typeof(CreateProjectHandler).Assembly));
        services.AddValidatorsFromAssemblyContaining<CreateProjectValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddCors(options =>
        {
            var origins = configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? [];

            options.AddPolicy("Client", policy =>
            {
                if (origins.Length == 0)
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    return;
                }

                policy.WithOrigins(origins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        services.AddJwtAuthentication(configuration);
        var signalR = services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = false;
            options.MaximumReceiveMessageSize = 128 * 1024;
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(45);
            options.KeepAliveInterval = TimeSpan.FromSeconds(15);
        });
        var signalRRedis = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(signalRRedis))
            signalR.AddStackExchangeRedis(signalRRedis);
        services.AddSwaggerDocumentation();
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("auth", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
        });

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
                options.Configuration = redisConnection);
        }

        return services;
    }

    private static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var key = configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(issuer) ||
            string.IsNullOrWhiteSpace(audience) ||
            string.IsNullOrWhiteSpace(key) ||
            Encoding.UTF8.GetByteCount(key) < 32)
        {
            throw new InvalidOperationException(
                "JWT Issuer, Audience, and a Key of at least 32 bytes must be configured.");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Query["access_token"];
                        if (!string.IsNullOrWhiteSpace(token) &&
                            context.HttpContext.Request.Path.StartsWithSegments("/hubs/collaboration"))
                            context.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }

    private static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Coding API",
                Version = "v1",
                Description = "Coding platform HTTP API."
            });

            var scheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter a valid JWT bearer token.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            };

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, scheme);
            options.OperationFilter<SwaggerAuthorizationOperationFilter>();
        });

        return services;
    }
}
