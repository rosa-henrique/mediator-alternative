using Mediator.Samples;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

services.AddLogging(config =>
{
    config.AddConsole();
    config.SetMinimumLevel(LogLevel.Information);
});

services.AddTransient<UserRepository>();

MediatorSample.Init(services);
Mediator.Samples.MediatorDecoratorPattern.Init(services);

var serviceProvider = services.BuildServiceProvider();

await MediatorSample.Call(serviceProvider);
await Mediator.Samples.MediatorDecoratorPattern.Call(serviceProvider);

public class UserRepository
{
    public void Save()
        => Console.WriteLine("Saving...");
}

