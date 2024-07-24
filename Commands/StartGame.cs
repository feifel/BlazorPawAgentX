using BlazorPawAgentX.Models;

namespace BlazorPawAgentX.Commands
{
    public class StartGame(int playerIndex) : AbstractCommand<StartGame, string>
    {
        public int PlayerIndex { get; set; } = playerIndex;
    }
}
