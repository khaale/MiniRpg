using Microsoft.Extensions.Options;
using MiniRpg.Core;
using MiniRpg.Domain.Commands.Handlers.Options;
using MiniRpg.Domain.Entities;
using MiniRpg.Domain.Services;

namespace MiniRpg.Domain.Commands.Handlers
{
    public class NewGameCommandHandler : ICommandHandler<NewGameCommand>
    {
        private readonly InitialPlayerStats _options;
        private readonly IPlayerStore _playerStore;

        public NewGameCommandHandler(
            IPlayerStore playerStore,
            IOptionsSnapshot<InitialPlayerStats> options
        )
        {
            _playerStore = playerStore;
            _options = options.Value;
        }

        public ExecutionResult Handle(NewGameCommand command)
        {
            var newPlayer = new Player(
                _options.MaxHealth,
                _options.Health,
                _options.Power,
                _options.Coins
            );

            _playerStore.SetPlayer(newPlayer);

            return ExecutionResult.Succeeded("New game started.. Enjoy!");
        }
    }
}