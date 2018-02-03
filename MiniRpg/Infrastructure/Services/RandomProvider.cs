using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MiniRpg.Core.Options;
using MiniRpg.Domain.Services;

namespace MiniRpg.Infrastructure.Services
{
    public class RandomProviderOptions : IGameOptions
    {
        public int? Seed { get; set; }

        public string Key => "Random";

        public List<string> Validate() => new List<string>();
    }

    public class RandomProvider : IRandom
    {
        private readonly Random _rnd;

        public RandomProvider(IOptionsSnapshot<RandomProviderOptions> options)
        {
            var seed = options.Value.Seed;

            _rnd = seed != null
                ? new Random(seed.Value)
                : new Random();
        }

        public double GetNext() => _rnd.NextDouble();
        public int GetInRange(int start, int end) => _rnd.Next(start, end + 1);
    }
}