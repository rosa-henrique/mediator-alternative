namespace Mediator.Abstractions;

public interface IMediator
{
    Task<TResponse> PublishAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    Task SendAsync(INotification notification, CancellationToken cancellationToken = default);
}