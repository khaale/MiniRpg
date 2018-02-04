using FluentAssertions;
using Microsoft.Extensions.Options;
using MiniRpg.Core.Commands;
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
    public abstract class PurchaseCommandHandlerFactBase<THandler, TCommand, TOptions>
        where THandler : ICommandHandler<TCommand>
        where TCommand : ICommand, new()
        where TOptions : PurchaseOptionsBase, new()
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
            result.Should().BeOfType<ErrorResult>();
            result.Message.Should().Contain("dead");
        }

        [Fact]
        public void ShouldFail_WhenNotEnoughCoins()
        {
            // arrange
            var options = new TOptions();
            var (originalPlayer, actualPlayer) = PlayerFixture.CreateClonedPlayers(coins: 0);

            var sut = CreateSut(
                player: actualPlayer,
                options: options);

            // act
            var result = sut.Handle(new TCommand());

            // assert
            result.Should().BeOfType<ErrorResult>();
        }

        protected (Player actual, TOptions options) MakePurchase(Player originalPlayer)
        {
            // arrange
            var player = PlayerFixture.ClonePlayer(originalPlayer);
            var options = new TOptions();
            var mockRandom = Substitute.For<IRandom>();
            mockRandom.GetInRange(1, options.MaxValue).Returns(options.MaxValue);

            var sut = CreateSut(
                player: player,
                random: mockRandom,
                options: options);

            // act
            var result = sut.Handle(new TCommand());

            // assert result
            result.Should().BeOfType<OkResult>(result.Message);
            // assert player state change
            return (player, options);
        }
        
        private THandler CreateSut(
            Player player = null,
            IPlayerStore playerStore = null,
            IRandom random = null,
            TOptions options = null)
        {
            player = player ?? PlayerFixture.CreateDefaultPlayer();
            playerStore = playerStore ?? Substitute.For<IPlayerStore>();
            playerStore.GetPlayer().Returns(player);
            random = random ?? Substitute.For<IRandom>();
            var opts = Substitute.For<IOptions<TOptions>>();
            opts.Value.Returns(options ?? new TOptions());

            return CreateSutImpl(playerStore, random, opts);
        }

        protected abstract THandler CreateSutImpl(
            IPlayerStore playerStore,
            IRandom random,
            IOptions<TOptions> opts);
    }

    public class PurchaseWeaponCommandHandlerFacts :
        PurchaseCommandHandlerFactBase<PurchaseWeaponCommandHandler, PurchaseWeaponCommand, PurchaseWeaponOptions>
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

        protected override PurchaseWeaponCommandHandler CreateSutImpl(IPlayerStore playerStore, IRandom random, IOptions<PurchaseWeaponOptions> opts)
        {
            return new PurchaseWeaponCommandHandler(playerStore, random, opts);
        }
    }

    public class PurchaseArmorCommandHandlerFacts :
        PurchaseCommandHandlerFactBase<PurchaseArmorCommandHandler, PurchaseArmorCommand, PurchaseArmorOptions>
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
        
        protected override PurchaseArmorCommandHandler CreateSutImpl(
            IPlayerStore playerStore,
            IRandom random,
            IOptions<PurchaseArmorOptions> opts)
        {
            return new PurchaseArmorCommandHandler(playerStore, random, opts);
        }
    }

    public class PurchaseHealingCommandHandlerFacts :
        PurchaseCommandHandlerFactBase<PurchaseHealingCommandHandler, PurchaseHealingCommand, PurchaseHealingOptions>
    {
        [Fact]
        public void ShouldIncreaseHealth_WhenPurchased()
        {
            var originalPlayer = PlayerFixture.CreateDefaultPlayer(maxHealth: 100, health: 20);

            var (actualPlayer, options) = MakePurchase(originalPlayer);

            actualPlayer.Health.Should()
                .Be(originalPlayer.Health + options.MaxValue);
            actualPlayer.Coins.Should()
                .Be(originalPlayer.Coins - options.Price);
        }

        [Fact]
        public void ShouldNotIncreaseHealthMoreThanMaxHealth_WhenPurchased()
        {
            var originalPlayer = PlayerFixture.CreateDefaultPlayer(maxHealth: 100, health: 95);

            var (actualPlayer, options) = MakePurchase(originalPlayer);

            actualPlayer.Health.Should()
                .Be(originalPlayer.MaxHealth);
            actualPlayer.Coins.Should()
                .Be(originalPlayer.Coins - options.Price);
        }
        
        protected override PurchaseHealingCommandHandler CreateSutImpl(
            IPlayerStore playerStore,
            IRandom random,
            IOptions<PurchaseHealingOptions> opts)
        {
            return new PurchaseHealingCommandHandler(playerStore, random, opts);
        }
    }
}