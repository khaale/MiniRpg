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
        private readonly PurchaseOptions _purchaseHealingHandlerOptions;
        private readonly PurchaseOptions _purchaseWeaponHandlerOptions;

        public BotCommandHandler(
            IPlayerStore playerStore,
            IRandom random,
            IFormulaCalculator calculator,
            IOptionsSnapshot<AttackOptions> attackOptions,
            IOptionsSnapshot<PurchaseOptions> purchaseWeaponOptions,
            IOptionsSnapshot<PurchaseOptions> purchaseHealingOptions
        ) : base(playerStore, random)
        {
            _calculator = calculator;
            _attackOptions = attackOptions.Value;
            _purchaseWeaponHandlerOptions = purchaseWeaponOptions.Get(PurchaseOptions.PurchaseWeaponKey);
            _purchaseHealingHandlerOptions = purchaseHealingOptions.Get(PurchaseOptions.PurchaseHealingKey);
        }

        protected override CommandResult HandlePlayerCommand(Player player)
        {
            // NOTE: taking into account that WIN health reduce ~ health, it does it make sense to have maximal health
            // Instead, we try to balance at the very boundary of FAIL health reduce, buying weapons (while it make sense) or just saving coins for bad times.

            // If we are at risk (i.e. next attack may kill us) - heal at first priority
            if (player.Health <= _attackOptions.LoseHealthReduce)
            {
                if (player.Coins >= _purchaseHealingHandlerOptions.Price)
                {
                    return new RedirectResult(new PurchaseHealingCommand(), "Health is too low, want to heal");
                }
            }

            // Otherwise it might make sense to by a weapon
            if (player.Coins >= _purchaseWeaponHandlerOptions.Price)
            {
                // Estimating if new weapon (with +1 bonus) will increase our win probablility
                var formulaContext = new FormulaContext(player);
                var currentProb = _calculator.Calculate(_attackOptions.WinProbFormula, formulaContext);
                formulaContext.PlayerPower = formulaContext.PlayerPower + 1;
                var nextProb = _calculator.Calculate(_attackOptions.WinProbFormula, formulaContext);
                if (nextProb > currentProb)
                {
                    return new RedirectResult(new PurchaseWeaponCommand(), "Looks like it worth byuing a weapon");
                }
            }

            return new RedirectResult(new AttackCommand(), "Well, let's kill somebody");
        }
    }
}