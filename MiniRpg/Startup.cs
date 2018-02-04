using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MiniRpg.Core.Commands;
using MiniRpg.Core.Options;
using MiniRpg.Domain.Commands;
using MiniRpg.Domain.Commands.Handlers;
using MiniRpg.Domain.Commands.Handlers.Options;
using MiniRpg.Domain.Services;
using MiniRpg.Infrastructure.Services;
using MiniRpg.Infrastructure.Services.Formula;

namespace MiniRpg
{
    public static class Startup
    {
        public static void ConfigureOptions(IServiceCollection sc, IConfiguration cfg)
        {
            void ConfigureGameOptions<T>() where T : class, IGameOptions
            {
                sc.Configure<T>(opt =>
                {
                    cfg.Bind(opt.Key, opt);
                });
            }

            sc.AddOptions();
            ConfigureGameOptions<RandomProviderOptions>();
            ConfigureGameOptions<InitialPlayerStats>();
            ConfigureGameOptions<AttackOptions>();
            ConfigureGameOptions<PurchaseWeaponOptions>();
            ConfigureGameOptions<PurchaseArmorOptions>();
            ConfigureGameOptions<PurchaseHealingOptions>();
        }

        public static void ConfigureServices(IServiceCollection sc)
        {
            sc.AddSingleton<IPlayerStore, PlayerStore>();
            sc.AddSingleton<IRandom, RandomProvider>();
            //sc.AddSingleton<IFormulaCalculator, DummyFormulaCalculator>();
            sc.AddSingleton<IFormulaCalculator, ScriptingFormulaCalculator>();

            sc.AddSingleton<ICommandHandler<NewGameCommand>, NewGameCommandHandler>();
            sc.AddSingleton<ICommandHandler<AttackCommand>, AttackCommandHandler>();
            sc.AddSingleton<ICommandHandler<PurchaseWeaponCommand>, PurchaseWeaponCommandHandler>();
            sc.AddSingleton<ICommandHandler<PurchaseArmorCommand>, PurchaseArmorCommandHandler>();
            sc.AddSingleton<ICommandHandler<PurchaseHealingCommand>, PurchaseHealingCommandHandler>();
            sc.AddSingleton<ICommandHandler<PurchaseArmorCommand>, PurchaseArmorCommandHandler>();
            sc.AddSingleton<ICommandHandler<BotCommand>, BotCommandHandler>();
            sc.AddSingleton<ICommandDispatcher, GameCommandDispatcher>();

            sc.AddSingleton<GameController>();
        }

        public static IReadOnlyCollection<IGameOptions> ResolveGameOptions(IServiceProvider sp)
        {
            return new List<IGameOptions>
            {
                sp.GetService<IOptions<RandomProviderOptions>>().Value,
                sp.GetService<IOptions<InitialPlayerStats>>().Value,
                sp.GetService<IOptions<AttackOptions>>().Value,
                sp.GetService<IOptions<PurchaseWeaponOptions>>().Value,
                sp.GetService<IOptions<PurchaseArmorOptions>>().Value,
                sp.GetService<IOptions<PurchaseHealingOptions>>().Value
            };
        }

        public static void WarmUpFormulaCalculator(IServiceProvider sp)
        {
            var calc = sp.GetService<IFormulaCalculator>();
            var options = sp.GetService<IOptionsSnapshot<AttackOptions>>();

            if (!(calc is ScriptingFormulaCalculator scriptCalc))
            {
                Console.WriteLine("Only scripting calc needed to be warmed up.");
                return;
            }

            scriptCalc.GetCompiledFormula(options.Value.WinProbFormula);
        }
    }
}