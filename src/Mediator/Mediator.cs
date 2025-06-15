using System.Collections.Concurrent;
using Mediator.Abstractions;

namespace Mediator;

public class Mediator(IServiceProvider serviceProvider) : IMediator 
{
    private static readonly ConcurrentDictionary<Type, Func<object, object, CancellationToken, Task<object>>>
        _requestsHandlerCache = new();

    private static readonly ConcurrentDictionary<Type, Func<object, object, CancellationToken, Task<object>>>
        _notificationsHandlerCache = new();

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var handlerType = typeof(IHandlerRequest<,>).MakeGenericType(requestType, typeof(TResponse));

        var invoker = _requestsHandlerCache.GetOrAdd(requestType, static reqType =>
        {
            var genericHandlerType = typeof(IHandlerRequest<,>).MakeGenericType(reqType, typeof(TResponse));
            var methodInfo = genericHandlerType.GetMethod("HandleAsync")!;

            return async (handlerObj, requestObj, ct) =>
            {
                var resultTask = methodInfo.Invoke(handlerObj, [requestObj, ct]) as Task ??
                                 throw new InvalidOperationException("Handler method did not return a Task.");
                ;

                await resultTask.ConfigureAwait(false);

                var resultProperty = resultTask.GetType().GetProperty("Result");
                return resultProperty?.GetValue(resultTask)!;
            };
        });

        var handler = serviceProvider.GetService(handlerType) ??
                      throw new InvalidOperationException($"Handler not found for {requestType.Name}");

        var result = await invoker(handler, request, cancellationToken);
        return (TResponse)result;
    }

    public async Task PublishAsync(INotification notification, CancellationToken cancellationToken = default)
    {
        var notificationType = notification.GetType();
        var handlerType = typeof(IHandlerNotification<>).MakeGenericType(notificationType);

        var invoker = _notificationsHandlerCache.GetOrAdd(notificationType, static reqType =>
        {
            var genericHandlerType = typeof(IHandlerNotification<>).MakeGenericType(reqType);
            var methodInfo = genericHandlerType.GetMethod("HandleAsync")!;

            return async (handlerObj, requestObj, ct) =>
            {
                var resultTask = methodInfo.Invoke(handlerObj, [requestObj, ct]) as Task ??
                                 throw new InvalidOperationException("Handler method did not return a Task.");
                ;

                await resultTask.ConfigureAwait(false);

                var resultProperty = resultTask.GetType().GetProperty("Result");
                return resultProperty?.GetValue(resultTask)!;
            };
        });

        var handler = serviceProvider.GetService(handlerType) ??
                      throw new InvalidOperationException($"Handler not found for {notificationType.Name}");

        await invoker(handler, notification, cancellationToken);
    }
}