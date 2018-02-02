using MiniRpg.Domain.Entities;

namespace MiniRpg.Domain.Services
{
    public class FormulaContext
    {
        public FormulaContext()
        {
        }

        public FormulaContext(Player player)
        {
            PlayerMaxHealth = player.MaxHealth;
            PlayerHealth = player.Health;
            PlayerPower = player.Power;
            PlayerCoins = player.Coins;
        }

        public int PlayerMaxHealth { get; set; }
        public int PlayerHealth { get; set; }
        public int PlayerPower { get; set; }
        public int PlayerCoins { get; set; }
    }
}