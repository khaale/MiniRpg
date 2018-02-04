using System.Collections.Generic;
using MiniRpg.Core.Options;

namespace MiniRpg.Domain.Commands.Handlers.Options
{
    public abstract class PurchaseOptionsBase : IGameOptions
    {
        public int Price { get; set; }
        public int MaxValue { get; set; }

        public abstract string Key { get; }
     
        public virtual List<string> Validate()
        {
            var validator = new Validator();

            validator.ShouldBeGreaterThanZero(() => Price);
            validator.ShouldBeGreaterThanZero(() => MaxValue);

            return validator.Errors;
        }
    }

    public class PurchaseWeaponOptions : PurchaseOptionsBase
    {
        public override string Key => "PurchaseWeapon";

        public PurchaseWeaponOptions()
        {
            Price = 10;
            MaxValue = 2;
        }
    }

    public class PurchaseArmorOptions : PurchaseOptionsBase
    {
        public override string Key => "PurchaseArmor";

        public PurchaseArmorOptions()
        {
            Price = 10;
            MaxValue = 2;
        }
    }

    public class PurchaseHealingOptions : PurchaseOptionsBase
    {
        public override string Key => "PurchaseHealing";

        public PurchaseHealingOptions()
        {
            Price = 3;
            MaxValue = 10;
        }
    }
}