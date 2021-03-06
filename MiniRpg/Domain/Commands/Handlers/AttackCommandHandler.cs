﻿using Microsoft.Extensions.Options;
using MiniRpg.Core.Commands;
using MiniRpg.Domain.Commands.Handlers.Options;
using MiniRpg.Domain.Entities;
using MiniRpg.Domain.Services;

namespace MiniRpg.Domain.Commands.Handlers
{
    public class AttackCommandHandler : PlayerCommandHandlerBase<AttackCommand>
    {
        private readonly IFormulaCalculator _calculator;
        private readonly AttackOptions _options;

        public AttackCommandHandler(
            IPlayerStore playerStore,
            IRandom random,
            IFormulaCalculator calculator,
            IOptions<AttackOptions> options)
            : base(playerStore, random)
        {
            _calculator = calculator;
            _options = options.Value;
        }

        protected override CommandResult HandlePlayerCommand(Player player)
        {
            var winProb = _calculator.Calculate(_options.WinProbFormula, new FormulaContext(player));
            var isWin = Random.GetNext() <= winProb;
            if (isWin)
            {
                player.ReduceHealthBy(_options.WinHealthReduceRate);
                player.GiveCoins(_options.WinBonusCoins);
            }
            else
            {
                player.ReduceHealth(_options.LoseHealthReduce);
            }
            return CommandResult.Ok(string.Format("Attack and {0}", isWin ? "WIN :)" : "LOSE :("));
        }
    }
}