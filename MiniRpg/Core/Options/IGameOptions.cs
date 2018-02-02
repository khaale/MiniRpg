using System.Collections.Generic;

namespace MiniRpg.Core.Options
{
    public interface IGameOptions
    {
        string Key { get; }
        List<string> Validate();
    }
}