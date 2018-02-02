using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MiniRpg.Core.Options;

namespace MiniRpg.Domain.Commands.Handlers.Options
{
    public class InitialPlayerStats : IGameOptions
    {
        public int MaxHealth { get; set; } = 100;
        public int Health { get; set; } = 100;
        public int Power { get; set; } = 1;
        public int Coins { get; set; } = 2;

        public string Key => "InitialPlayerStats";

        public List<string> Validate()
        {
            var validator = new Validator();

            validator.ShouldBeGreaterThanZero(() => MaxHealth);
            validator.ShouldBeValid(() => Health, x => x > 0 && x <= MaxHealth, "should be in interval (1, MaxHealth]");
            validator.ShouldBeGreaterOrEqualThanZero(() => Power);
            validator.ShouldBeGreaterOrEqualThanZero(() => Coins);

            return validator.Errors;
        }
    }
}