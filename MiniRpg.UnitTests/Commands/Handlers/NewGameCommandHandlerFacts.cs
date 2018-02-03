using FluentAssertions;
using Microsoft.Extensions.Options;
using MiniRpg.Core;
using MiniRpg.Domain.Commands;
using MiniRpg.Domain.Commands.Handlers;
using MiniRpg.Domain.Commands.Handlers.Options;
using MiniRpg.Domain.Entities;
using MiniRpg.Domain.Services;
using NSubstitute;
using Xunit;

namespace MiniRpg.UnitTests.Commands.Handlers
{
    public class NewGameCommandHandlerFacts
    {
        [Fact]
        public void ShouldSetNewPlayer()
        {
            // arrange
            Player newPlayer = null;
            var options = new InitialPlayerStats();
            var mockPlayerStore = Substitute.For<IPlayerStore>();
            mockPlayerStore.SetPlayer(Arg.Do<Player>(x => newPlayer = x));

            var sut = CreateSut(mockPlayerStore, options);

            // act
            var result = sut.Handle(new NewGameCommand());

            // assert
            result.Should().BeOfType<OkResult>();

            mockPlayerStore.Received().SetPlayer(Arg.Any<Player>());

            newPlayer.Should().NotBeNull();
            newPlayer.MaxHealth.Should().Be(options.MaxHealth);
            newPlayer.Health.Should().Be(options.Health);
            newPlayer.Power.Should().Be(options.Power);
            newPlayer.Coins.Should().Be(options.Coins);
        }
        
        private static NewGameCommandHandler CreateSut(IPlayerStore playerStore, InitialPlayerStats options)
        {
            var optionsSnapshot = Substitute.For<IOptionsSnapshot<InitialPlayerStats>>();
            optionsSnapshot.Value.Returns(options);
            
            return new NewGameCommandHandler(playerStore, optionsSnapshot);
        }
    }
}
