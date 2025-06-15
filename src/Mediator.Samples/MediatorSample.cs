using System.Reflection;
using Mediator.Abstractions;
using Mediator.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mediator.Samples;

public static class MediatorSample
{
    public static void Init(this IServiceCollection services)
    {
        services.AddTransient<IMediator, Mediator>();
        services.AddMediator(typeof(Program).Assembly);
    }

    public static async Task Call(this IServiceProvider serviceProvider)
    {
        Console.WriteLine("START SAMPLE");
        var mediator = serviceProvider.GetRequiredService<IMediator>();

        var request = new CreateUserRequest { Username = "teste", Password = "12345566" };
        var result = await mediator.SendAsync(request);

        Console.WriteLine(result);
        Console.WriteLine("END SAMPLE");
    }
}

public class CreateUserRequest : IRequest<string>
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public class CreateUserRequestHandlerRequestHandler(
    UserRepository accountRepository,
    IMediator mediator,
    ILogger<CreateUserRequestHandlerRequestHandler> logger) : IRequestHandler<CreateUserRequest, string>
{
    public async Task<string> HandleAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"Creating user {request.Username}...");
        accountRepository.Save();

        var notification = new UserCreatedEvent(request.Username);
        await mediator.PublishAsync(notification);

        return $"{request.Username} created";
    }
}

public record UserCreatedEvent(string Username) : INotification;

public class UserCreatedEventRequest(ILogger<UserCreatedEventRequest> logger) : INotificationHandler<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation($"notification user {request.Username}...");

        return Task.CompletedTask;
    }
}