namespace Mediator.Abstractions;

public interface IHandlerNotification<in TNotification>
    where TNotification : INotification
{
    Task HandleAsync(TNotification request, CancellationToken cancellationToken = default);
}