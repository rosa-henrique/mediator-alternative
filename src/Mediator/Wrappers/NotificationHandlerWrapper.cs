using Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator.Wrappers;

public abstract class NotificationHandlerWrapper
{
    public abstract Task Handle(INotification notification, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

public class NotificationHandlerWrapperImpl<TNotification> : NotificationHandlerWrapper
    where TNotification : INotification
{

    public override Task Handle(INotification notification, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        Task Handler(CancellationToken t = default) => serviceProvider
            .GetRequiredService<INotificationHandler<TNotification>>()
            .HandleAsync((TNotification)notification, t == default ? cancellationToken : t);

        return Handler();
    }
}