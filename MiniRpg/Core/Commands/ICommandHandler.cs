namespace MiniRpg.Core.Commands
{
    public interface ICommandHandler<T> where T : ICommand
    {
        CommandResult Handle(T command);
    }
}