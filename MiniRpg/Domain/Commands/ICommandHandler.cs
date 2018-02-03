using MiniRpg.Core;

namespace MiniRpg.Domain.Commands
{
    public interface ICommandHandler<T> where T : ICommand
    {
        CommandResult Handle(T command);
    }
}