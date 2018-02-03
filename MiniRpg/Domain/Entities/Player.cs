using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniRpg.Domain.Entities
{
    public class Player
    {
        private readonly List<Item> _items;

        public Player(int netMaxHealth, int health, int netPower, int coins, IEnumerable<Item> items = null)
        {
            NetMaxHealth = netMaxHealth;
            NetPower = netPower;
            Health = health;
            Coins = coins;
            _items = items?.ToList() ?? new List<Item>();
        }

        public int Health { get; private set; }

        public int Coins { get; private set; }

        /// <summary>
        /// Playser's power without bonuses
        /// </summary>
        public int NetPower { get; }

        /// <summary>
        /// Player's max health without bonuses
        /// </summary>
        public int NetMaxHealth { get; }

        public IReadOnlyCollection<Item> Items => _items;

        public bool IsDead => Health <= 0;

        /// <summary>
        /// Effective player's power (with bonuses from items)
        /// </summary>
        public int Power => NetPower + _items.Where(i => i.Type == ItemType.Weapon).Sum(i => i.Bonus);

        /// <summary>
        /// Effective player's max health (with bonuses from items)
        /// </summary>
        public int MaxHealth => NetMaxHealth + _items.Where(i => i.Type == ItemType.Armor).Sum(i => i.Bonus);

        public void ReduceHealthBy(double rate)
        {
            if (rate <= 0 || rate > 1)
            {
                throw new InvalidOperationException($"{nameof(rate)} should be in interval (0;1].");
            }

            // TODO: add to assumptions
            var decrement = Math.Max(1, (int) (Health * rate));
            Health = Math.Max(0, Health - decrement);
        }

        public void GiveCoins(int count)
        {
            if (count <= 0)
            {
                throw new InvalidOperationException($"{nameof(count)} should be greater than 0.");
            }

            Coins = Coins + count;
        }

        public void GiveItem(Item item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _items.Add(item);
        }

        public void ReduceHealth(int damage)
        {
            if (damage <= 0)
            {
                throw new InvalidOperationException($"{nameof(damage)} should be greater than 0.");
            }

            Health = Math.Max(0, Health - damage);
        }

        public bool TryWithdrawal(int coins)
        {
            if (coins <= 0)
            {
                throw new InvalidOperationException($"{nameof(coins)} should be greater than 0.");
            }

            if (Coins - coins < 0)
            {
                return false;
            }
            Coins -= coins;
            return true;
        }

        public void IncreaseHealth(int healthBonus)
        {
            if (healthBonus <= 0)
            {
                throw new InvalidOperationException($"{nameof(healthBonus)} should be greater than 0.");
            }

            Health = Math.Min(MaxHealth, Health + healthBonus);
        }
    }
}