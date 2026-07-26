using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Coding.Application.Behaviors;

public sealed class RequestLoggingBehavior<TRequest, TResponse>(
    ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();
            logger.LogInformation(
                "Handled application request {RequestName} in {ElapsedMilliseconds} ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Application request {RequestName} failed after {ElapsedMilliseconds} ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
