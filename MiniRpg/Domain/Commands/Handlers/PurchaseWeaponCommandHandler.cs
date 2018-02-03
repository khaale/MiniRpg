using Microsoft.Extensions.Options;
using MiniRpg.Core;
using MiniRpg.Domain.Commands.Handlers.Options;
using MiniRpg.Domain.Entities;
using MiniRpg.Domain.Services;

namespace MiniRpg.Domain.Commands.Handlers
{
    public class PurchaseWeaponCommandHandler : CommandHandlerBase<PurchaseWeaponCommand>
    {
        private readonly PurchaseOptions _options;

        public PurchaseWeaponCommandHandler(
            IPlayerStore playerStore,
            IRandom random,
            IOptionsSnapshot<PurchaseOptions> options
        ) : base(playerStore, random) => _options = options.Get(PurchaseOptions.PurchaseWeaponKey);

        protected override CommandResult HandleImpl(Player player)
        {
            if (!player.TryWithdrawal(_options.Price))
            {
                return CommandResult.Error("Not enough coins to buy a weapon. Go and kill monsters!");
            }

            var bonus = Random.GetInRange(1, _options.MaxValue);
            var item = new Item(ItemType.Weapon, bonus);
            player.GiveItem(item);

            return CommandResult.Ok($"Purchased a weapon with power bonus {bonus}");
        }
    }
}