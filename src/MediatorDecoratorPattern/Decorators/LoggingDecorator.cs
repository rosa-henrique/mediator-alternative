using ErrorOr;
using MediatorDecoratorPattern.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediatorDecoratorPattern.Decorators;

public static class LoggingDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<ErrorOr<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            logger.LogInformation("Processing command {Command}", commandName);

            ErrorOr<TResponse> result = await innerHandler.Handle(command, cancellationToken);

            if (!result.IsError)
            {
                logger.LogInformation("Completed command {Command}", commandName);
            }
            else
            {
                logger.LogError("Completed command {Command} with error", commandName);
            }

            return result;
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<ErrorOr<Success>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            logger.LogInformation("Processing command {Command}", commandName);

            ErrorOr<Success> result = await innerHandler.Handle(command, cancellationToken);

            if (!result.IsError)
            {
                logger.LogInformation("Completed command {Command}", commandName);
            }
            else
            {
                logger.LogError("Completed command {Command} with error", commandName);
            }

            return result;
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<ErrorOr<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            string queryName = typeof(TQuery).Name;

            logger.LogInformation("Processing query {Query}", queryName);

            ErrorOr<TResponse> result = await innerHandler.Handle(query, cancellationToken);

            if (!result.IsError)
            {
                logger.LogInformation("Completed query {Query}", queryName);
            }
            else
            {
                logger.LogError("Completed query {Query} with error", queryName);
            }

            return result;
        }
    }
}