using FluentAssertions;
using MiniRpg.Core.Commands;
using MiniRpg.Domain.Commands;
using NSubstitute;
using Xunit;

namespace MiniRpg.UnitTests
{
    public class GameCommandDispatcherFacts
    {
        [Fact]
        public void Handle_ShouldCallRedirectToCommandAndAddRedirectMessageToFinalResult_WhenRedirected()
        {
            // arrange
            var mockBotHandler = Substitute.For<ICommandHandler<BotCommand>>();
            mockBotHandler.Handle(null)
                .ReturnsForAnyArgs(CommandResult.Redirect(new PurchaseHealingCommand(), "Redirected"));

            var mockPurchaseHealingHandler = Substitute.For<ICommandHandler<PurchaseHealingCommand>>();
            mockPurchaseHealingHandler.Handle(null)
                .ReturnsForAnyArgs(CommandResult.Ok("Healed"));

            var sut = CreateSut(purchaseHealingHandler: mockPurchaseHealingHandler, botHandler: mockBotHandler);

            // act
            var result = sut.Handle(new BotCommand());

            // assert
            mockBotHandler.Received(1).Handle(Arg.Any<BotCommand>());
            mockPurchaseHealingHandler.Received(1).Handle(Arg.Any<PurchaseHealingCommand>());
            result.IsOk.Should().BeTrue();
            result.Message.Should().Contain("Redirected");
            result.Message.Should().Contain("Healed");
        }

        private static GameCommandDispatcher CreateSut(
            ICommandHandler<NewGameCommand> newGameHandler = null,
            ICommandHandler<AttackCommand> attackHandler = null,
            ICommandHandler<PurchaseWeaponCommand> purchaseWeaponHandler = null,
            ICommandHandler<PurchaseArmorCommand> purchaseArmorHandler = null,
            ICommandHandler<PurchaseHealingCommand> purchaseHealingHandler = null,
            ICommandHandler<BotCommand> botHandler = null)
        {
            var sut = new GameCommandDispatcher(
                newGameHandler ?? Substitute.For<ICommandHandler<NewGameCommand>>(),
                attackHandler ?? Substitute.For<ICommandHandler<AttackCommand>>(),
                purchaseWeaponHandler ?? Substitute.For<ICommandHandler<PurchaseWeaponCommand>>(),
                purchaseArmorHandler ?? Substitute.For<ICommandHandler<PurchaseArmorCommand>>(),
                purchaseHealingHandler ?? Substitute.For<ICommandHandler<PurchaseHealingCommand>>(),
                botHandler ?? Substitute.For<ICommandHandler<BotCommand>>());
            return sut;
        }
    }
}