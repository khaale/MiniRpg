using System;
using MiniRpg.Core.Commands;
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
            var mockCommandDispatcher = Substitute.For<ICommandDispatcher>();
            var sut = CreateSut(mockCommandDispatcher);
            // act
            sut.HandleKey(key);
            // assert
            mockCommandDispatcher.Received()
                .Handle(Arg.Is<ICommand>(cmd => cmd.GetType() == type));
        }

        [Fact]
        public void StartNew_ShouldExecuteNewGameCommand()
        {
            // arrange
            var mockCommandDispatcher = Substitute.For<ICommandDispatcher>();
            var sut = CreateSut(mockCommandDispatcher);

            // act
            sut.StartNew();

            // assert
            mockCommandDispatcher.Received()
                .Handle(Arg.Any<NewGameCommand>());
            ;
        }

        private static GameController CreateSut(ICommandDispatcher commandDispatcher)
        {
            var sut = new GameController(commandDispatcher);
            return sut;
        }
    }
}