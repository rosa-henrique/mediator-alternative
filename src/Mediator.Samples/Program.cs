using Mediator.Abstractions;
using Mediator.Extensions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddTransient<IMediator, Mediator.Mediator>();
services.AddTransient<UserRepository>();
services.AddMediator(typeof(Program).Assembly);


var serviceProvider = services.BuildServiceProvider();
var mediator = serviceProvider.GetRequiredService<IMediator>();

var request = new CreateUserRequest { Username = "teste", Password = "12345566" };
var result = await mediator.PublishAsync(request);

Console.WriteLine(result);

public class UserRepository
{
    public void Save()
        => Console.WriteLine("Saving...");
}

public class CreateUserRequest : IRequest<string>
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public class CreateUserHandlerRequestHandlerRequest(UserRepository accountRepository, IMediator mediator) : IHandlerRequest<CreateUserRequest, string>
{
    public async Task<string> HandleAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Creating user {request.Username}...");
        accountRepository.Save();

        var notification = new UserCreatedEvent(request.Username);
        await mediator.SendAsync(notification);
        
        return $"{request.Username} created";
    }
}

public record UserCreatedEvent(string Username) : INotification;

public class UserCreatedEventRequest : IHandlerNotification<UserCreatedEvent>
{
    public Task HandleAsync(UserCreatedEvent request, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"notification user {request.Username}...");
            
        return Task.CompletedTask;
    }
}

