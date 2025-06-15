using System.Windows.Input;
using ErrorOr;
using MediatorDecoratorPattern.Abstractions;
using MediatorDecoratorPattern.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Mediator.Samples;

public static class MediatorDecoratorPattern
{
    public static void Init(this IServiceCollection services)
    {
        services.AddMediator(typeof(Program));
    }

    public static async Task Call(this IServiceProvider serviceProvider)
    {
        Console.WriteLine("START DECORATOR PATTERN");
        var handler = serviceProvider.GetRequiredService<ICommandHandler<CreateUserCommand, string>>();
        
        var request = new CreateUserCommand { Username = "teste", Password = "12345566" };
        var result = await handler.Handle(request, default);
            
        Console.WriteLine(result.Value);
        Console.WriteLine("END DECORATOR PATTERN");
    }
}

public sealed class CreateUserCommand : ICommand<string>
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public class CreateUserCommandHandler(UserRepository userRepository) : ICommandHandler<CreateUserCommand, string>
{
    public Task<ErrorOr<string>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Creating user {command.Username}...");
        userRepository.Save();
        
        return Task.FromResult<ErrorOr<string>>($"{command.Username} created");
    }
}