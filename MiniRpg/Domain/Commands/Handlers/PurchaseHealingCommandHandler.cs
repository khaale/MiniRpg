﻿using Microsoft.Extensions.Options;
using MiniRpg.Core;
using MiniRpg.Domain.Commands.Handlers.Options;
using MiniRpg.Domain.Entities;
using MiniRpg.Domain.Services;

namespace MiniRpg.Domain.Commands.Handlers
{
    public class PurchaseHealingCommandHandler : CommandHandlerBase<PurchaseHealingCommand>
    {
        private readonly PurchaseOptions _options;

        public PurchaseHealingCommandHandler(
            IPlayerStore playerStore,
            IRandom random,
            IOptionsSnapshot<PurchaseOptions> options
        ) : base(playerStore, random) => _options = options.Get(PurchaseOptions.PurchaseHealingKey);

        protected override ExecutionResult HandleImpl(Player player)
        {
            if (!player.TryWithdrawal(_options.Price))
            {
                return ExecutionResult.Failed("Not enough coins for healing. Go and kill monsters!");
            }

            var bonus = _options.MaxValue;
            player.IncreaseHealth(bonus);

            return ExecutionResult.Succeeded($"Healed on {bonus} poins");
        }
    }
}