namespace MiniRpg.Domain.Commands
{
    public interface ICommand
    {
    }

    public sealed class NewGameCommand : ICommand
    {
    }

    public sealed class AttackCommand : ICommand
    {
    }

    public sealed class PurchaseWeaponCommand : ICommand
    {
    }

    public sealed class PurchaseArmorCommand : ICommand
    {
    }

    public sealed class PurchaseHealingCommand : ICommand
    {
    }

    public sealed class BotCommand : ICommand
    {
    }
}