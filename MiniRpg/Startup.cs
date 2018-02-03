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
            void ConfigurePurchaseOption(string key)
            {
                sc.Configure<PurchaseOptions>(key, opt =>
                {
                    opt.ConfigureDefaults(key);
                    cfg.Bind(key, opt);
                });
            }

            sc.AddOptions();
            sc.Configure<RandomProviderOptions>(cfg.GetSection("Random"));
            sc.Configure<InitialPlayerStats>(cfg.GetSection("InitialPlayerStats"));
            sc.Configure<AttackOptions>(cfg.GetSection("Attack"));
            ConfigurePurchaseOption(PurchaseOptions.PurchaseWeaponKey);
            ConfigurePurchaseOption(PurchaseOptions.PurchaseArmorKey);
            ConfigurePurchaseOption(PurchaseOptions.PurchaseHealingKey);

            sc.Configure<PurchaseOptions>("PurchaseWeapon", cfg.GetSection("PurchaseWeapon"));
            sc.Configure<PurchaseOptions>("PurchaseArmor", cfg.GetSection("PurchaseArmor"));
            sc.Configure<PurchaseOptions>("PurchaseHealing", cfg.GetSection("PurchaseHealing"));
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
                sp.GetService<IOptionsSnapshot<RandomProviderOptions>>().Value,
                sp.GetService<IOptionsSnapshot<InitialPlayerStats>>().Value,
                sp.GetService<IOptionsSnapshot<AttackOptions>>().Value,
                sp.GetService<IOptionsSnapshot<PurchaseOptions>>().Get(PurchaseOptions.PurchaseWeaponKey),
                sp.GetService<IOptionsSnapshot<PurchaseOptions>>().Get(PurchaseOptions.PurchaseArmorKey),
                sp.GetService<IOptionsSnapshot<PurchaseOptions>>().Get(PurchaseOptions.PurchaseHealingKey)
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