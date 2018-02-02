using System;
using MiniRpg.Domain.Entities;
using MiniRpg.Domain.Services;

namespace MiniRpg.Infrastructure.Services
{
    public class PlayerStore : IPlayerStore
    {
        private Player _player;

        public Player GetPlayer() =>
            _player ?? throw new InvalidOperationException("Player should be set prior the call.");

        public void SetPlayer(Player player) => _player = player;
    }
}