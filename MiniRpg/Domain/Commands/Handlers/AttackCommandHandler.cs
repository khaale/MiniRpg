using Microsoft.Extensions.Options;
using MiniRpg.Core;
using MiniRpg.Domain.Commands.Handlers.Options;
using MiniRpg.Domain.Entities;
using MiniRpg.Domain.Services;

namespace MiniRpg.Domain.Commands.Handlers
{
    public class AttackCommandHandler : CommandHandlerBase<AttackCommand>
    {
        private readonly IFormulaCalculator _calculator;
        private readonly AttackOptions _options;

        public AttackCommandHandler(
            IPlayerStore playerStore,
            IRandom random,
            IFormulaCalculator calculator,
            IOptionsSnapshot<AttackOptions> options)
            : base(playerStore, random)
        {
            _calculator = calculator;
            _options = options.Value;
        }

        protected override ExecutionResult HandleImpl(Player player)
        {
            if (player.IsDead)
            {
                return ExecutionResult.Failed("Dead man cannot attack (well, zombie can, but not you).");
            }
            var winProb = _calculator.Calculate(_options.WinProbFormula, new FormulaContext(player));
            var isWin = Random.GetNext() <= winProb;
            if (isWin)
            {
                player.ReduceHealthBy(_options.WinHealthReduceRate);
                player.GiveCoins(_options.WinBonusCoins);
            }
            else
            {
                player.ReduceHealth(_options.LooseHealthReduce);
            }
            return ExecutionResult.Succeeded(string.Format("Attack and {0}", isWin ? "WIN :)" : "LOOSE :("));
        }
    }
}