using System;
using System.Collections.Generic;
using MiniRpg.Core.Options;

namespace MiniRpg.Domain.Commands.Handlers.Options
{
    public class PurchaseOptions : IGameOptions
    {
        public const string PurchaseWeaponKey = "PurchaseWeapon";
        public const string PurchaseArmorKey = "PurchaseArmor";
        public const string PurchaseHealingKey = "PurchaseHealing";

        private string _key;

        public int Price { get; set; }
        public int MaxValue { get; set; }

        public void ConfigureDefaults(string key)
        {
            _key = key;
            switch (key)
            {
                case PurchaseWeaponKey:
                    Price = 10;
                    MaxValue = 2;
                    break;
                case PurchaseArmorKey:
                    Price = 10;
                    MaxValue = 2;
                    break;
                case PurchaseHealingKey:
                    Price = 3;
                    MaxValue = 10;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unexpected key: '{key}'");
            }
        }

        public string Key =>
            _key ?? throw new InvalidOperationException("Purchase options should be configured before usage.");

        public List<string> Validate()
        {
            var validator = new Validator();

            validator.ShouldBeGreaterThanZero(() => Price);
            validator.ShouldBeGreaterThanZero(() => MaxValue);

            return validator.Errors;
        }
    }
}