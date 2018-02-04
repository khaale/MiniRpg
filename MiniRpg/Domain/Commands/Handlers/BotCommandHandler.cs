using Microsoft.Extensions.Options;
using MiniRpg.Core.Commands;
using MiniRpg.Domain.Commands.Handlers.Options;
using MiniRpg.Domain.Entities;
using MiniRpg.Domain.Services;

namespace MiniRpg.Domain.Commands.Handlers
{
    public class BotCommandHandler : PlayerCommandHandlerBase<BotCommand>
    {
        private readonly AttackOptions _attackOptions;
        private readonly IFormulaCalculator _calculator;
        private readonly PurchaseHealingOptions _purchaseHealingHandlerOptions;
        private readonly PurchaseWeaponOptions _purchaseWeaponHandlerOptions;

        public BotCommandHandler(
            IPlayerStore playerStore,
            IRandom random,
            IFormulaCalculator calculator,
            IOptions<AttackOptions> attackOptions,
            IOptions<PurchaseWeaponOptions> purchaseWeaponOptions,
            IOptions<PurchaseHealingOptions> purchaseHealingOptions
        ) : base(playerStore, random)
        {
            _calculator = calculator;
            _attackOptions = attackOptions.Value;
            _purchaseWeaponHandlerOptions = purchaseWeaponOptions.Value;
            _purchaseHealingHandlerOptions = purchaseHealingOptions.Value;
        }

        protected override CommandResult HandlePlayerCommand(Player player)
        {
            // NOTE: taking into account that WIN health reduce ~ health, it does it make sense to have maximal health
            // Instead, we try to balance at the very boundary of FAIL health reduce, buying weapons (while it make sense) or just saving coins for bad times.

            // Player state indicators
            // Use functions instead of direct values, because some indicators might be relatively 'heavy'
            // and also to try new C# features :)
            bool LowHealth() => player.Health <= _attackOptions.LoseHealthReduce;
            bool CanPurchaseHealing() => player.Coins >= _purchaseHealingHandlerOptions.Price;
            bool NeedWeapon()
            {
                // Estimating if new weapon (with +1 bonus) will increase our win probablility
                var formulaContext = new FormulaContext(player);
                var currentProb = _calculator.Calculate(_attackOptions.WinProbFormula, formulaContext);
                formulaContext.PlayerPower = formulaContext.PlayerPower + 1;
                var nextProb = _calculator.Calculate(_attackOptions.WinProbFormula, formulaContext);
                return nextProb > currentProb;
            }
            bool CanPurchaseWeapon() => player.Coins >= _purchaseWeaponHandlerOptions.Price;

            RedirectResult result;
            // If we are at risk (i.e. next attack may kill us) - heal at first priority
            if (LowHealth() && CanPurchaseHealing())
            {
                result = new RedirectResult(new PurchaseHealingCommand(), "Health is too low, want to heal");
            }
            // Otherwise it might make sense to by a weapon
            else if (CanPurchaseWeapon() && NeedWeapon())
            {
                result = new RedirectResult(new PurchaseWeaponCommand(), "Looks like it worth byuing a weapon");
            }
            else
            {
                result = new RedirectResult(new AttackCommand(), "Well, let's kill somebody");
            }
            return result;
        }
    }
}