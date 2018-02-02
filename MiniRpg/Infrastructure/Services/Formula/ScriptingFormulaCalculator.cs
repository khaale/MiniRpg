using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using MiniRpg.Domain.Services;

namespace MiniRpg.Infrastructure.Services.Formula
{
    public class ScriptingFormulaCalculator : IFormulaCalculator
    {
        private readonly Dictionary<string, ScriptRunner<double>> _runnerCache =
            new Dictionary<string, ScriptRunner<double>>();

        public double Calculate(string formula, FormulaContext context)
        {
            var runner = GetCompiledFormula(formula);

            double result;

            try
            {
                result = runner(context).Result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error when executing '{formula}'.", ex);
            }

            if (result < 0 || result > 1)
            {
                throw new ApplicationException($"Calculated formula result '{result}' should be in range [0,1]");
            }

            return result;
        }

        public ScriptRunner<double> GetCompiledFormula(string formula)
        {
            if (_runnerCache.TryGetValue(formula, out var runner))
            {
                return runner;
            }

            var script = CSharpScript.Create<double>(
                formula,
                globalsType: typeof(FormulaContext),
                options: ScriptOptions.Default.WithImports("System.Math")
            );

            runner = script.CreateDelegate();
            _runnerCache.Add(formula, runner);
            return runner;
        }
    }
}