using System;
using MiniRpg.Domain.Services;

namespace MiniRpg.Infrastructure.Services.Formula
{
    public class DummyFormulaCalculator : IFormulaCalculator
    {
        public double Calculate(string text, FormulaContext context) =>
            Math.Min(0.4 + context.PlayerPower * 0.05, 0.7);
    }
}