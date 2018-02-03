using MiniRpg.Domain.Entities;

namespace MiniRpg.UnitTests
{
    public class PlayerFixture
    {
        public static (Player original, Player actual) CreateClonedPlayers(
            int coins = 100, 
            int maxHealth = 100,
            int health = 100)
        {
            var player = CreateDefaultPlayer(coins, maxHealth, health);
            return (player, ClonePlayer(player));
        }

        public static Player CreateDefaultPlayer(int coins = 100, int maxHealth = 100, int health = 100) =>
            new Player(maxHealth, health, 1, coins);

        public static Player ClonePlayer(Player original) =>
            new Player(original.NetMaxHealth, original.Health, original.NetPower, original.Coins, original.Items);
    }
}