using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MiniRpg
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Mini RPG game!");

            // Setup configuration and DI
            var sp = CreateServiceProvider();
            // Ensure that configuration is valid
            if (!IsConfigurationValid(sp))
            {
                Console.WriteLine("Unable to continue with invalid configuration. Exiting game..");
                return;
            }
            // Compiling text formula to script takes quite a long time, 
            // so doing warming-up and showing configuration at the same time (to 'hide' a pause from user).
            LoadGame(sp);
            // Run main game cycle
            var gameController = sp.GetService<GameController>();
            RunGame(gameController);
        }

        private static void RunGame(GameController gameController)
        {
            var result = gameController.StartNew();
            ConsoleUtils.PrintExecutionResult(result);
            ConsoleUtils.PrintInitialState(gameController.QueryPlayerState());

            while (true)
            {
                // read keypress
                var key = ConsoleUtils.ReadKey();

                result = gameController.HandleKey(key);
                ConsoleUtils.PrintExecutionResult(result);

                if (result.IsOk)
                {
                    var player = gameController.QueryPlayerState();
                    ConsoleUtils.PrintState(player);

                    // handling player's death
                    if (player.IsDead)
                    {
                        ConsoleUtils.PrintWithColor(ConsoleColor.DarkRed, () => Console.WriteLine("You are dead!"));
                        gameController.StartNew();
                        ConsoleUtils.PrintInitialState(player);
                    }
                }
            }
        }

        private static void LoadGame(IServiceProvider sp)
        {
            var allOptions = Startup.ResolveGameOptions(sp)
                .Select(x => (x.Key, (object) x))
                .ToArray();

            Console.WriteLine("Script engine is initializing.. Let's look on the configuration for now:");

            Task.WaitAll(
                Task.Run(() => Startup.WarmUpFormulaCalculator(sp)),
                ConsoleUtils.PrintOptionsWithDelayAsync(300, allOptions));
        }

        private static bool IsConfigurationValid(IServiceProvider sp)
        {
            var errors = Startup.ResolveGameOptions(sp)
                .SelectMany(opt => opt.Validate().Select(err => (OptionKey: opt.Key, ErrorMessage: err)))
                .ToList();

            if (errors.Any())
            {
                ConsoleUtils.PrintWithColor(ConsoleColor.Red, () =>
                    errors.ForEach(optionError =>
                        Console.WriteLine($"Error in {optionError.OptionKey}: {optionError.ErrorMessage}")));

                return false;
            }

            return true;
        }

        private static IServiceProvider CreateServiceProvider()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var cfg = builder.Build();

            var sc = new ServiceCollection();
            Startup.ConfigureOptions(sc, cfg);
            Startup.ConfigureServices(sc);
            return sc.BuildServiceProvider();
        }
    }
}