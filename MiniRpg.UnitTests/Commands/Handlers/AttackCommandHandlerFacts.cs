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
    public class AttackCommandHandlerFacts
    {
        [Fact]
        public void ShouldFail_WhenPlayerIsDead()
        {
            // arrange
            var player = new Player(0,0,0,0);
            var sut = CreateSut(player);
            // act
            var result = sut.Handle(new AttackCommand());
            // assert
            result.Should().BeOfType<FailResult>();
            result.Message.Should().Contain("dead");
        }

        [Fact]
        public void ShouldCallFormulaCalculator_WithFormulaFromOptions()
        {
            // arrange
            var (_, actualPlayer) = PlayerFixture.CreateClonedPlayers();
            var mockCalculator = Substitute.For<IFormulaCalculator>();
            var options = new AttackOptions { WinProbFormula = "formula" };

            var sut = CreateSut(
                player: actualPlayer,
                calculator: mockCalculator,
                options: options);
            // act
            sut.Handle(new AttackCommand());
            // assert
            mockCalculator.Received().Calculate("formula", Arg.Any<FormulaContext>());
        }

        [Fact]
        public void ShouldWin_WhenRandomValueNotGreaterThanWinProbability()
        {
            // arrange
            var options = new AttackOptions();
            var (originalPlayer, actualPlayer) = PlayerFixture.CreateClonedPlayers();
            var (mockCalculator, mockRandom) = SetupRandomAndCalc(0.7, 0.7);

            var sut = CreateSut(
                player: actualPlayer,
                calculator: mockCalculator,
                random: mockRandom,
                options: options);
            // act
            var result = sut.Handle(new AttackCommand());
            // assert result
            result.Should().BeOfType<SuccessResult>();
            result.Message.Should().ContainEquivalentOf("win");
            // assert player state change
            actualPlayer.Health.Should()
                .Be(originalPlayer.Health - (int) (options.WinHealthReduceRate * originalPlayer.Health));
            actualPlayer.Coins.Should()
                .Be(originalPlayer.Coins + options.WinBonusCoins);
        }

        [Fact]
        public void ShouldLoose_WhenRandomValueGreaterThanWinProbability()
        {
            // arrange
            var options = new AttackOptions();
            var (originalPlayer, actualPlayer) = PlayerFixture.CreateClonedPlayers();
            var (mockCalculator, mockRandom) = SetupRandomAndCalc(0.71, 0.7);

            var sut = CreateSut(
                player: actualPlayer,
                calculator: mockCalculator,
                random: mockRandom,
                options: options);
            // act
            var result = sut.Handle(new AttackCommand());
            // assert result
            result.Should().BeOfType<SuccessResult>();
            result.Message.Should().ContainEquivalentOf("lose");
            // assert player state change
            actualPlayer.Health.Should()
                .Be(originalPlayer.Health - options.LoseHealthReduce);
        }

        private static (IFormulaCalculator,IRandom) SetupRandomAndCalc(double randomValue, double calcValue)
        {
            var mockCalculator = Substitute.For<IFormulaCalculator>();
            mockCalculator.Calculate(null, null).ReturnsForAnyArgs(calcValue);
            var mockRandom = Substitute.For<IRandom>();
            mockRandom.GetNext().Returns(randomValue);
            return (mockCalculator, mockRandom);
        }

        private static AttackCommandHandler CreateSut(
            Player player = null, 
            IPlayerStore playerStore = null,
            IRandom random = null,
            IFormulaCalculator calculator = null,
            AttackOptions options = null)
        {
            player = player ?? PlayerFixture.CreateDefaultPlayer();
            playerStore = playerStore ?? Substitute.For<IPlayerStore>();
            playerStore.GetPlayer().Returns(player);
            random = random ?? Substitute.For<IRandom>();
            calculator = calculator ?? Substitute.For<IFormulaCalculator>();

            var optionsSnapshot = Substitute.For<IOptionsSnapshot<AttackOptions>>();
            optionsSnapshot.Value.Returns(options ?? new AttackOptions());
            
            return new AttackCommandHandler(playerStore, random, calculator, optionsSnapshot);
        }
    }
}
