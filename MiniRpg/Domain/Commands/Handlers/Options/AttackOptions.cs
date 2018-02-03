using System.Collections.Generic;
using MiniRpg.Core.Options;

namespace MiniRpg.Domain.Commands.Handlers.Options
{
    public class AttackOptions : IGameOptions
    {
        public string WinProbFormula { get; set; } = "Min(0.4 + PlayerPower * 0.05, 0.7)";
        public double WinHealthReduceRate { get; set; } = 0.1;
        public int WinBonusCoins { get; set; } = 10;
        public int LoseHealthReduce { get; set; } = 40;

        public string Key => "Attack";

        public List<string> Validate()
        {
            var validator = new Validator();

            validator.ShouldBeValid(() => WinProbFormula, x => !string.IsNullOrWhiteSpace(x), "should be provided");
            validator.ShouldBeValid(() => WinHealthReduceRate, x => WinHealthReduceRate > 0 && WinHealthReduceRate <= 1,
                "should be in interval (0,1]");
            validator.ShouldBeGreaterThanZero(() => WinBonusCoins);
            validator.ShouldBeGreaterThanZero(() => LoseHealthReduce);

            return validator.Errors;
        }
    }
}