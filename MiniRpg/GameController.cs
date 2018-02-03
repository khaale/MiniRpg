using System;
using System.Collections.Generic;
using MiniRpg.Core.Commands;
using MiniRpg.Domain.Commands;

namespace MiniRpg
{
    public class GameController
    {
        private readonly ICommandDispatcher _dispatcher;

        private static readonly IReadOnlyDictionary<ConsoleKey, ICommand> KeyCommandMap =
            new Dictionary<ConsoleKey, ICommand>
            {
                {ConsoleKey.W, new AttackCommand()},
                {ConsoleKey.A, new PurchaseWeaponCommand()},
                {ConsoleKey.D, new PurchaseArmorCommand()},
                {ConsoleKey.S, new PurchaseHealingCommand()},
                {ConsoleKey.E, new BotCommand()}
            };

        public GameController(
            ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public ExecutionResult HandleKey(ConsoleKey key)
        {
            if (!KeyCommandMap.TryGetValue(key, out var command))
            {
                return ExecutionResult.Error($"Unknown keyboard key: {(char) key}");
            }

            return _dispatcher.Handle(command);
        }

        public ExecutionResult StartNew()
        {
            return _dispatcher.Handle(new NewGameCommand());
        }
    }
}