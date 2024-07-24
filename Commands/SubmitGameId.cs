using System.Text.Json;

namespace BlazorPawAgentX.Commands
{
    public class SubmitGameId(string gameId) : AbstractCommand<SubmitGameId, string>
    {
        public string GameId { get; } = gameId;
        //public SubmitGameId(string gameId) : base("SubmitGameId")
        //{
        //    GameId = gameId;
        //}

        //public override string Serialize()
        //{
        //    return JsonSerializer.Serialize(this);
        //}
    }
}
