using MiniRpg.Core.Commands;
using MiniRpg.Domain.Entities;
using MiniRpg.Domain.Services;

namespace MiniRpg.Domain.Commands.Handlers
{
    public abstract class PlayerCommandHandlerBase<T> : ICommandHandler<T> where T : ICommand
    {
        protected IPlayerStore PlayerStore;
        protected IRandom Random;

        protected PlayerCommandHandlerBase(IPlayerStore playerStore, IRandom random)
        {
            PlayerStore = playerStore;
            Random = random;
        }

        public CommandResult Handle(T command)
        {
            var player = PlayerStore.GetPlayer();

            return player.IsDead
                ? CommandResult.Error("You are dead, sorry.")
                : HandlePlayerCommand(PlayerStore.GetPlayer());
        }

        protected abstract CommandResult HandlePlayerCommand(Player getPlayer);
    }
}