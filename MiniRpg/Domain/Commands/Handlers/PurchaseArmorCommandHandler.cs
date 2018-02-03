using Microsoft.Extensions.Options;
using MiniRpg.Core.Commands;
using MiniRpg.Domain.Commands.Handlers.Options;
using MiniRpg.Domain.Entities;
using MiniRpg.Domain.Services;

namespace MiniRpg.Domain.Commands.Handlers
{
    public class PurchaseArmorCommandHandler : PlayerCommandHandlerBase<PurchaseArmorCommand>
    {
        private readonly PurchaseOptions _options;

        public PurchaseArmorCommandHandler(
            IPlayerStore playerStore,
            IRandom random,
            IOptionsSnapshot<PurchaseOptions> options
        ) : base(playerStore, random) => _options = options.Get(PurchaseOptions.PurchaseArmorKey);

        protected override CommandResult HandlePlayerCommand(Player player)
        {
            if (!player.TryWithdrawal(_options.Price))
            {
                return CommandResult.Error("Not enough coins to buy an armor. Go and kill monsters!");
            }

            var bonus = Random.GetInRange(1, _options.MaxValue);
            var item = new Item(ItemType.Armor, bonus);
            player.GiveItem(item);

            return CommandResult.Ok($"Purchased an armor with max health bonus {bonus}");
        }
    }
}