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

        private readonly Dictionary<Type, Func<ICommand, ExecutionResult>> _handlers;

        public GameController(
            ICommandHandler<NewGameCommand> newGameHandler,
            ICommandHandler<AttackCommand> attackHandler,
            ICommandHandler<PurchaseWeaponCommand> purchaseWeaponHandler,
            ICommandHandler<PurchaseArmorCommand> purchaseArmorHandler,
            ICommandHandler<PurchaseHealingCommand> purchaseHealingHandler,
            ICommandHandler<BotCommand> botHandler)
        {
            _handlers = new Dictionary<Type, Func<ICommand, ExecutionResult>>();
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
                return ExecutionResult.Failed($"Unknown keyboard key: {(char) key}");
            }

            RedirectResult lastRedirect = null;
            do
            {
                var result = ExecuteCommand(command);

                if (lastRedirect != null)
                {
                    result.ApplyRedirect(lastRedirect);
                }

                // exit there if we were not redirected 
                if (!(result is RedirectResult redirect))
                {
                    return result;
                }

                command = redirect.RedirectToCommand;
                lastRedirect = redirect;
            } while (true);
        }

        public ExecutionResult StartNew()
        {
            return ExecuteCommand(new NewGameCommand());
        }

        private void RegisterHandler<T>(ICommandHandler<T> handler) where T : ICommand
        {
            _handlers.Add(typeof(T), cmd => handler.Handle((T) cmd));
        }

        private ExecutionResult ExecuteCommand(ICommand command)
        {
            if (!_handlers.TryGetValue(command.GetType(), out var handler))
            {
                throw new ArgumentException($"Command of type '{command.GetType()}' was not registered.");
            }

            return handler(command);
        }
    }
}