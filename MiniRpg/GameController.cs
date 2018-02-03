using System;
using System.Collections.Generic;
using MiniRpg.Core;
using MiniRpg.Domain.Commands;

namespace MiniRpg
{
    public class GameController
    {
        private static readonly IReadOnlyDictionary<ConsoleKey, ICommand> KeyCommandMap =
            new Dictionary<ConsoleKey, ICommand>
            {
                {ConsoleKey.W, new AttackCommand()},
                {ConsoleKey.A, new PurchaseWeaponCommand()},
                {ConsoleKey.D, new PurchaseArmorCommand()},
                {ConsoleKey.S, new PurchaseHealingCommand()},
                {ConsoleKey.E, new BotCommand()}
            };

        private readonly Dictionary<Type, Func<ICommand, CommandResult>> _handlers;

        public GameController(
            ICommandHandler<NewGameCommand> newGameHandler,
            ICommandHandler<AttackCommand> attackHandler,
            ICommandHandler<PurchaseWeaponCommand> purchaseWeaponHandler,
            ICommandHandler<PurchaseArmorCommand> purchaseArmorHandler,
            ICommandHandler<PurchaseHealingCommand> purchaseHealingHandler,
            ICommandHandler<BotCommand> botHandler)
        {
            _handlers = new Dictionary<Type, Func<ICommand, CommandResult>>();
            RegisterHandler(newGameHandler);
            RegisterHandler(attackHandler);
            RegisterHandler(purchaseWeaponHandler);
            RegisterHandler(purchaseArmorHandler);
            RegisterHandler(purchaseHealingHandler);
            RegisterHandler(botHandler);
        }

        public ExecutionResult HandleKey(ConsoleKey key)
        {
            if (!KeyCommandMap.TryGetValue(key, out var command))
            {
                return ExecutionResult.Error($"Unknown keyboard key: {(char) key}");
            }

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

        public ExecutionResult StartNew()
        {
            return ExecutionResult.FromCommand(ExecuteCommand(new NewGameCommand()));
        }

        private void RegisterHandler<T>(ICommandHandler<T> handler) where T : ICommand
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