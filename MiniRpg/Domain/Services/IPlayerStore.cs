using MiniRpg.Domain.Entities;

namespace MiniRpg.Domain.Services
{
    public interface IPlayerStore
    {
        Player GetPlayer();
        void SetPlayer(Player player);
    }
}