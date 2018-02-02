namespace MiniRpg.Domain.Entities
{
    public enum ItemType
    {
        Weapon,
        Armor
    }

    public class Item
    {
        public Item(ItemType type, int bonus)
        {
            Type = type;
            Bonus = bonus;
        }

        public ItemType Type { get; }
        public int Bonus { get; }
    }
}