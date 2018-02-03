using System;
using System.Collections.Generic;

namespace MiniRpg.Core.Commands
{
    public abstract class CommandDispatcherBase : ICommandDispatcher
    {
        private readonly Dictionary<Type, Func<ICommand, CommandResult>> _handlers;

        protected CommandDispatcherBase()
        {
            _handlers = new Dictionary<Type, Func<ICommand, CommandResult>>();
        }

        public ExecutionResult Handle(ICommand command)
        {
            var result = ExecutionResult.Default();
            do
            {
                var commandResult = ExecuteCommand(command);
                result.ApplyCommandResult(commandResult);

                // exit there if we were not redirected 
                if (!(commandResult is RedirectResult commandRedirectResult))
                {
                    return result;
                }

                command = commandRedirectResult.RedirectToCommand;
            } while (true);
        }

        protected void RegisterHandler<T>(ICommandHandler<T> handler) where T : ICommand
        {
            _handlers.Add(typeof(T), cmd => handler.Handle((T) cmd));
        }

        private CommandResult ExecuteCommand(ICommand command)
        {
            if (!_handlers.TryGetValue(command.GetType(), out var handler))
            {
                throw new ArgumentException($"Command of type '{command.GetType()}' was not registered.");
            }

            return handler(command);
        }
    }
}