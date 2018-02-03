using MiniRpg.Core.Commands;

namespace MiniRpg.Domain.Commands
{
    public class GameCommandDispatcher : CommandDispatcherBase
    {
        public GameCommandDispatcher(
            ICommandHandler<NewGameCommand> newGameHandler,
            ICommandHandler<AttackCommand> attackHandler,
            ICommandHandler<PurchaseWeaponCommand> purchaseWeaponHandler,
            ICommandHandler<PurchaseArmorCommand> purchaseArmorHandler,
            ICommandHandler<PurchaseHealingCommand> purchaseHealingHandler,
            ICommandHandler<BotCommand> botHandler)
        {
            RegisterHandler(newGameHandler);
            RegisterHandler(attackHandler);
            RegisterHandler(purchaseWeaponHandler);
            RegisterHandler(purchaseArmorHandler);
            RegisterHandler(purchaseHealingHandler);
            RegisterHandler(botHandler);
        }
    }
}