namespace MiniRpg.Core.Commands
{
    /// <summary>
    /// Represents a single command execution result
    /// </summary>
    public abstract class CommandResult
    {
        public static OkResult Ok(string message) => new OkResult(message);
        public static ErrorResult Error(string message) => new ErrorResult(message);

        public static RedirectResult Redirect(ICommand redirectToCommand, string message) =>
            new RedirectResult(redirectToCommand, message);

        protected CommandResult(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public class OkResult : CommandResult
    {
        public OkResult(string message) : base(message)
        {
        }
    }

    public class ErrorResult : CommandResult
    {
        public ErrorResult(string message) : base(message)
        {
        }
    }

    public class RedirectResult : CommandResult
    {
        public RedirectResult(ICommand redirectToCommand, string message) : base(message) =>
            RedirectToCommand = redirectToCommand;

        public ICommand RedirectToCommand { get; }
    }
}