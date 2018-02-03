using System;
using System.Linq;
using System.Threading.Tasks;
using MiniRpg.Core.Commands;
using MiniRpg.Domain.Entities;
using MiniRpg.Domain.Services;
using Newtonsoft.Json;

namespace MiniRpg
{
    public class ConsoleUtils
    {
        public static ConsoleKey ReadKey()
        {
            Console.Write("Print any of (w - attack|a - weapon|d - armor|s - heal|e - auto) keys: ");
            var input = Console.ReadKey();
            return input.Key;
        }

        public static void PrintInitialState(IPlayerStore playerStore)
        {
            PrintWithColor(ConsoleColor.Yellow, () =>
            {
                Console.WriteLine("-------------------");
                Console.Write("Your initial stats - ");
                PrintState(playerStore.GetPlayer());
            });
        }

        public static void PrintState(Player player)
        {
            var inventory = string.Join(", ", player.Items.Select(x => $"{x.Type} +{x.Bonus}"));
            Console.WriteLine(
                $"MaxHealth: {player.MaxHealth} | Health: {player.Health}| Power: {player.Power} | Coins: {player.Coins} | Inventory: {inventory}");
        }

        public static void PrintExecutionResult(ExecutionResult result)
        {
            var color = result.IsOk
                ? ConsoleColor.DarkGreen
                : ConsoleColor.Red;
            PrintWithColor(color, () => Console.WriteLine(Environment.NewLine + result.Message));
        }

        public static void PrintWithColor(ConsoleColor color, Action printAction)
        {
            var origColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            printAction();
            Console.ForegroundColor = origColor;
        }

        public static async Task PrintOptionsWithDelayAsync(
            int delayBetweenOptions,
            params (string Name, object Option)[] namedOptions)
        {
            foreach (var namedOption in namedOptions)
            {
                var optionsText = JsonConvert.SerializeObject(namedOption.Option, Formatting.Indented);
                await Console.Out.WriteLineAsync($"{namedOption.Name} - {optionsText}");
                await Task.Delay(delayBetweenOptions);
            }
        }
    }
}