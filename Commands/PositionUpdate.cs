using BlazorPawAgentX.Models;

namespace BlazorPawAgentX.Commands
{
    public class PositionUpdate(params Player[] players) : AbstractCommand<PositionUpdate, string>
    {
        public Player[] Players { get; } = players;
    }
}
