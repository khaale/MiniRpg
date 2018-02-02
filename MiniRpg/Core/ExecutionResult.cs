using MiniRpg.Domain.Commands;

namespace MiniRpg.Core
{
    public abstract class ExecutionResult
    {
        public static SuccessResult Succeeded(string message) => new SuccessResult(message);
        public static FailResult Failed(string message) => new FailResult(message);
        public static RedirectResult RedirectResult(ICommand redirectToCommand, string message) => new RedirectResult(redirectToCommand, message);

        protected ExecutionResult(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }

        public void ApplyRedirect(RedirectResult redirectResult)
        {
            Message = $"{redirectResult.Message} -> {Message}";
        }
    }

    public class SuccessResult : ExecutionResult
    {
        public SuccessResult(string message) : base(message)
        {
        }
    }

    public class FailResult : ExecutionResult
    {
        public FailResult(string message) : base(message)
        {
        }
    }

    public class RedirectResult : ExecutionResult
    {
        public RedirectResult(ICommand redirectToCommand, string message) : base(message)
        {
            RedirectToCommand = redirectToCommand;
        }

        public ICommand RedirectToCommand { get; }
    }


}