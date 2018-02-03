using MiniRpg.Core;

namespace MiniRpg.Domain.Commands
{
    /// <summary>
    /// Represents a command or commands sequence execution result
    /// </summary>
    public class ExecutionResult
    {
        public static ExecutionResult Default() => new ExecutionResult(false, "");

        public static ExecutionResult Ok(string message) => new ExecutionResult(true, message);

        public static ExecutionResult Error(string message) => new ExecutionResult(false, message);

        public static ExecutionResult FromCommand(CommandResult commandResult)
        {
            var result = new ExecutionResult(false, null);
            result.ApplyCommandResult(commandResult);
            return result;
        }

        private ExecutionResult(bool isOk, string message)
        {
            IsOk = isOk;
            Message = message;
        }

        public bool IsOk { get; private set; }

        public string Message { get; private set; }

        public void ApplyCommandResult(CommandResult commandResult)
        {
            IsOk = (commandResult is OkResult);
            Message += $" -> {commandResult.Message}";
        }
    }
}