using System.Collections.Generic;
using MiniRpg.Domain.Entities;

namespace MiniRpg.Domain.ReadModels
{
    public class PlayerReadModel
    {
        public PlayerReadModel()
        {
        }

        public PlayerReadModel(Player player)
        {
            MaxHealth = player.MaxHealth;
            Health = player.Health;
            Power = player.Power;
            Coins = player.Coins;
            Items = player.Items;
            IsDead = player.IsDead;
        }

        public bool IsDead { get; set; }
        public IReadOnlyCollection<Item> Items { get; set; }
        public int Coins { get; set; }
        public int Power { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
    }
}