using System.Net.Sockets;
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
    /// <summary>
    /// Experementing with generic test suites
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    public abstract class PurchaseCommandHandlerFactBase<THandler, TCommand>
        where THandler : ICommandHandler<TCommand>
        where TCommand : ICommand, new()
    {

        [Fact]
        public void ShouldFail_WhenPlayerIsDead()
        {
            // arrange
            var player = new Player(0, 0, 0, 0);
            var sut = CreateSut(player);
            // act
            var result = sut.Handle(new TCommand());
            // assert
            result.Should().BeOfType<FailResult>();
            result.Message.Should().Contain("dead");
        }

        [Fact]
        public void ShouldFail_WhenNotEnoughCoins()
        {
            // arrange
            var options = CreateDefaultOptions();
            var (originalPlayer, actualPlayer) = PlayerFixture.CreateClonedPlayers(coins: 0);

            var sut = CreateSut(
                player: actualPlayer,
                options: options);

            // act
            var result = sut.Handle(new TCommand());

            // assert
            result.Should().BeOfType<FailResult>();
        }
        
        protected (Player actual, PurchaseOptions options) MakePurchase(Player originalPlayer)
        {
            // arrange
            var player = PlayerFixture.ClonePlayer(originalPlayer);
            var options = CreateDefaultOptions();
            var mockRandom = Substitute.For<IRandom>();
            mockRandom.GetInRange(1, options.MaxValue).Returns(options.MaxValue);

            var sut = CreateSut(
                player: player,
                random: mockRandom,
                options: options);

            // act
            var result = sut.Handle(new TCommand());

            // assert result
            result.Should().BeOfType<SuccessResult>(result.Message);
            // assert player state change
            return (player, options);
        }

        private PurchaseOptions CreateDefaultOptions()
        {
            var options = new PurchaseOptions();
            options.ConfigureDefaults(PurchaseOptionsKey);
            return options;
        }

        private THandler CreateSut(
            Player player = null,
            IPlayerStore playerStore = null,
            IRandom random = null,
            PurchaseOptions options = null)
        {
            player = player ?? PlayerFixture.CreateDefaultPlayer();
            playerStore = playerStore ?? Substitute.For<IPlayerStore>();
            playerStore.GetPlayer().Returns(player);
            random = random ?? Substitute.For<IRandom>();
            var optionsSnapshot = Substitute.For<IOptionsSnapshot<PurchaseOptions>>();
            optionsSnapshot.Get(PurchaseOptionsKey).Returns(options ?? CreateDefaultOptions());

            return CreateSutImpl(playerStore, random, optionsSnapshot);
        }

        protected abstract THandler CreateSutImpl(
            IPlayerStore playerStore, 
            IRandom random,
            IOptionsSnapshot<PurchaseOptions> optionsSnapshot);
        
        protected abstract string PurchaseOptionsKey { get; }
    }

    public class PurchaseWeaponCommandHandlerFacts : 
        PurchaseCommandHandlerFactBase<PurchaseWeaponCommandHandler,PurchaseWeaponCommand>
    {
        [Fact]
        public void ShouldIncreasePower_WhenPurchase()
        {
            var originalPlayer = PlayerFixture.CreateDefaultPlayer();

            var (actualPlayer, options) = MakePurchase(originalPlayer);

            actualPlayer.Power.Should()
                .Be(originalPlayer.Power + options.MaxValue);
            actualPlayer.Coins.Should()
                .Be(originalPlayer.Coins - options.Price);
        }

        protected override string PurchaseOptionsKey => PurchaseOptions.PurchaseWeaponKey;

        protected override PurchaseWeaponCommandHandler CreateSutImpl(
            IPlayerStore playerStore, 
            IRandom random, 
            IOptionsSnapshot<PurchaseOptions> optionsSnapshot)
        {
            return new PurchaseWeaponCommandHandler(playerStore, random, optionsSnapshot);
        }
    }

    public class PurchaseArmorCommandHandlerFacts :
        PurchaseCommandHandlerFactBase<PurchaseArmorCommandHandler, PurchaseArmorCommand>
    {
        [Fact]
        public void ShouldIncreaseMaxHealth_WhenPurchase()
        {
            var originalPlayer = PlayerFixture.CreateDefaultPlayer();

            var (actualPlayer, options) = MakePurchase(originalPlayer);


            actualPlayer.MaxHealth.Should()
                .Be(originalPlayer.MaxHealth + options.MaxValue);
            actualPlayer.Coins.Should()
                .Be(originalPlayer.Coins - options.Price);
        }

        protected override string PurchaseOptionsKey => PurchaseOptions.PurchaseArmorKey;

        protected override PurchaseArmorCommandHandler CreateSutImpl(
            IPlayerStore playerStore,
            IRandom random,
            IOptionsSnapshot<PurchaseOptions> optionsSnapshot)
        {
            return new PurchaseArmorCommandHandler(playerStore, random, optionsSnapshot);
        }
    }

    public class PurchaseHealingCommandHandlerFacts :
        PurchaseCommandHandlerFactBase<PurchaseHealingCommandHandler, PurchaseHealingCommand>
    {
        [Fact]
        public void ShouldIncreaseHealth_WhenPurchased()
        {
            var originalPlayer = PlayerFixture.CreateDefaultPlayer(maxHealth:100, health: 20);

            var (actualPlayer, options) = MakePurchase(originalPlayer);

            actualPlayer.Health.Should()
                .Be(originalPlayer.Health + options.MaxValue);
            actualPlayer.Coins.Should()
                .Be(originalPlayer.Coins - options.Price);
        }

        [Fact]
        public void ShouldNotIncreaseHealthMoreThanMaxHealth_WhenPurchased()
        {
            var originalPlayer = PlayerFixture.CreateDefaultPlayer(maxHealth:100,  health: 95);

            var (actualPlayer, options) = MakePurchase(originalPlayer);

            actualPlayer.Health.Should()
                .Be(originalPlayer.MaxHealth);
            actualPlayer.Coins.Should()
                .Be(originalPlayer.Coins - options.Price);
        }

        protected override string PurchaseOptionsKey => PurchaseOptions.PurchaseHealingKey;

        protected override PurchaseHealingCommandHandler CreateSutImpl(
            IPlayerStore playerStore,
            IRandom random,
            IOptionsSnapshot<PurchaseOptions> optionsSnapshot)
        {
            return new PurchaseHealingCommandHandler(playerStore, random, optionsSnapshot);
        }


    }
}
