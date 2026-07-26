using Coding.Application.Abstractions;
using MediatR;

namespace Coding.Application.Behaviors;

public sealed class CacheInvalidationBehavior<TRequest, TResponse>(
    ICacheService cache,
    ICurrentUser currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();
        if (typeof(TRequest).Name.EndsWith("Command", StringComparison.Ordinal))
        {
            cache.Remove($"dashboard:user:{currentUser.UserId:N}");
            cache.RemoveByPrefix($"analytics:user:{currentUser.UserId:N}:");
        }

        return response;
    }
}
