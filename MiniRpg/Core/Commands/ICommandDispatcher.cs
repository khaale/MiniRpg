namespace MiniRpg.Core.Commands
{
    public interface ICommandDispatcher
    {
        ExecutionResult Handle(ICommand command);
    }
}