namespace Mediator.Abstractions;

public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    Task HandleAsync(TNotification request, CancellationToken cancellationToken = default);
}