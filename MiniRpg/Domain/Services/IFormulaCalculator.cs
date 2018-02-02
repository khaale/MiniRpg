namespace MiniRpg.Domain.Services
{
    public interface IFormulaCalculator
    {
        double Calculate(string formula, FormulaContext player);
    }
}