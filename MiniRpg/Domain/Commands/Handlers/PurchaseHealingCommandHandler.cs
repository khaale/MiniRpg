﻿using Microsoft.Extensions.Options;
using MiniRpg.Core.Commands;
using MiniRpg.Domain.Commands.Handlers.Options;
using MiniRpg.Domain.Entities;
using MiniRpg.Domain.Services;

namespace MiniRpg.Domain.Commands.Handlers
{
    public class PurchaseHealingCommandHandler : PlayerCommandHandlerBase<PurchaseHealingCommand>
    {
        private readonly PurchaseOptions _options;

        public PurchaseHealingCommandHandler(
            IPlayerStore playerStore,
            IRandom random,
            IOptionsSnapshot<PurchaseOptions> options
        ) : base(playerStore, random) => _options = options.Get(PurchaseOptions.PurchaseHealingKey);

        protected override CommandResult HandlePlayerCommand(Player player)
        {
            if (!player.TryWithdrawal(_options.Price))
            {
                return CommandResult.Error("Not enough coins for healing. Go and kill monsters!");
            }

            var bonus = _options.MaxValue;
            player.IncreaseHealth(bonus);

            return CommandResult.Ok($"Purchased a {bonus} point(s) healing portion.");
        }
    }
}