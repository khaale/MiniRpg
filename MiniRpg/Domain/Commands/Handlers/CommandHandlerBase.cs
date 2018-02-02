using MiniRpg.Core;
using MiniRpg.Domain.Entities;
using MiniRpg.Domain.Services;

namespace MiniRpg.Domain.Commands.Handlers
{
    public abstract class CommandHandlerBase<T> : ICommandHandler<T> where T : ICommand
    {
        protected IPlayerStore PlayerStore;
        protected IRandom Random;

        protected CommandHandlerBase(IPlayerStore playerStore, IRandom random)
        {
            PlayerStore = playerStore;
            Random = random;
        }

        public ExecutionResult Handle(T command)
        {
            var player = PlayerStore.GetPlayer();

            return player.IsDead
                ? ExecutionResult.Failed("You are dead, sorry.")
                : HandleImpl(PlayerStore.GetPlayer());
        }

        protected abstract ExecutionResult HandleImpl(Player getPlayer);
    }
}