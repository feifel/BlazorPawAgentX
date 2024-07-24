using System.Text.Json;

namespace BlazorPawAgentX.Commands
{
    public class JoinGame(string playerName) : AbstractCommand<JoinGame, String>
    {
        public string PlayerName { get; } = playerName;
    }
}
