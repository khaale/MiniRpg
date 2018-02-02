using MiniRpg.Core;

namespace MiniRpg.Domain.Commands
{
    public interface ICommandHandler<T> where T : ICommand
    {
        ExecutionResult Handle(T command);
    }
}