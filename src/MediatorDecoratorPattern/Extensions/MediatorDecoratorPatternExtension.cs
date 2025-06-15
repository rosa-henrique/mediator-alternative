using MediatorDecoratorPattern.Abstractions;
using MediatorDecoratorPattern.Decorators;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorDecoratorPattern.Extensions;

public static class MediatorDecoratorPatternExtension
{
    public static IServiceCollection AddMediator(this IServiceCollection services, params Type[] types)
    {
        services.Scan(scan => scan.FromAssembliesOf([..types, typeof(MediatorDecoratorPatternExtension)])
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        
        //services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        //services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

        
        //remove on dev
        return services;
    }
}