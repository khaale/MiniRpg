using System;
using FluentAssertions;
using MiniRpg.Core;
using MiniRpg.Domain.Commands;
using NSubstitute;
using Xunit;

namespace MiniRpg.UnitTests
{
    public class GameControllerFacts
    {
        [Theory]
        [InlineData(ConsoleKey.W, typeof(AttackCommand))]
        [InlineData(ConsoleKey.A, typeof(PurchaseWeaponCommand))]
        [InlineData(ConsoleKey.D, typeof(PurchaseArmorCommand))]
        [InlineData(ConsoleKey.S, typeof(PurchaseHealingCommand))]
        [InlineData(ConsoleKey.E, typeof(BotCommand))]
        public void HandleKey_ShouldCallProperHandlers(ConsoleKey key, Type type)
        {
            // arrange
            // It's hard to assert calls using NSubstitute functionality when generic arguments are not known in compile time
            // So use a custom mock verification 'framework'
            object handledCommand = null;
            int callCount = 0;

            void HandleCall(object cmd)
            {
                handledCommand = cmd;
                callCount++;
            }

            var mockAttackHandler = Substitute.For<ICommandHandler<AttackCommand>>();
            mockAttackHandler.Handle(Arg.Do<AttackCommand>(HandleCall));
            var mockPurchaseWeaponHandler = Substitute.For<ICommandHandler<PurchaseWeaponCommand>>();
            mockPurchaseWeaponHandler.Handle(Arg.Do<PurchaseWeaponCommand>(HandleCall));
            var mockPurchaseArmorHandler = Substitute.For<ICommandHandler<PurchaseArmorCommand>>();
            mockPurchaseArmorHandler.Handle(Arg.Do<PurchaseArmorCommand>(HandleCall));
            var mockPurchaseHealingHandler = Substitute.For<ICommandHandler<PurchaseHealingCommand>>();
            mockPurchaseHealingHandler.Handle(Arg.Do<PurchaseHealingCommand>(HandleCall));
            var mockBotHandler = Substitute.For<ICommandHandler<BotCommand>>();
            mockBotHandler.Handle(Arg.Do<BotCommand>(HandleCall));

            var sut = CreateSut(
                attackHandler: mockAttackHandler, 
                purchaseWeaponHandler: mockPurchaseWeaponHandler, 
                purchaseArmorHandler: mockPurchaseArmorHandler, 
                purchaseHealingHandler: mockPurchaseHealingHandler, 
                botHandler: mockBotHandler);

            // act
            sut.HandleKey(key);

            // assert
            callCount.Should().Be(1, "exactly only one command should be called");
            handledCommand.Should().BeOfType(type, $"command of type '{type.Name}' should be called on key '{key}'");
        }

        [Fact]
        public void HandleKey_ShouldCallRedirectToCommandAndAddRedirectMessageToFinalResult_WhenRedirected()
        {
            // arrange
            var mockBotHandler = Substitute.For<ICommandHandler<BotCommand>>();
            mockBotHandler.Handle(null)
                .ReturnsForAnyArgs(ExecutionResult.RedirectResult(new PurchaseHealingCommand(), "Redirected"));
            var mockPurchaseHealingHandler = Substitute.For<ICommandHandler<PurchaseHealingCommand>>();
            mockPurchaseHealingHandler.Handle(null)
                .ReturnsForAnyArgs(ExecutionResult.Succeeded("Healed"));
            var sut = CreateSut(purchaseHealingHandler: mockPurchaseHealingHandler, botHandler: mockBotHandler);
            // act
            var result = sut.HandleKey(ConsoleKey.E);
            // assert
            mockBotHandler.Received(1).Handle(Arg.Any<BotCommand>());
            mockPurchaseHealingHandler.Received(1).Handle(Arg.Any<PurchaseHealingCommand>());
            result.Should().BeOfType<SuccessResult>();
            result.Message.Should().Contain("Redirected");
            result.Message.Should().Contain("Healed");
        }

        [Fact]
        public void StartNew_ShouldExecuteNewGameCommand()
        {
            // arrange
            var mockNewGameHandler = Substitute.For<ICommandHandler<NewGameCommand>>();
            var sut = CreateSut(newGameHandler: mockNewGameHandler);
            // act
            sut.StartNew();
            // assert
            mockNewGameHandler.Received(1).Handle(Arg.Any<NewGameCommand>());
        }

        private static GameController CreateSut(
            ICommandHandler<NewGameCommand> newGameHandler = null,
            ICommandHandler<AttackCommand> attackHandler = null,
            ICommandHandler<PurchaseWeaponCommand> purchaseWeaponHandler = null,
            ICommandHandler<PurchaseArmorCommand> purchaseArmorHandler = null,
            ICommandHandler<PurchaseHealingCommand> purchaseHealingHandler = null,
            ICommandHandler<BotCommand> botHandler = null)
        {
            var sut = new GameController(
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
