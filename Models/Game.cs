using BlazorPawAgentX.Commands;
using Microsoft.AspNetCore.SignalR.Client;
using System.Timers;
using Timer = System.Timers.Timer;

namespace BlazorPawAgentX.Models
{
    public class Game
    {
        public static int UpdateIntervalAgent = 30;
        public static int UpdateIntervalPlayers = 3;
        public static string[] PlayerColors = new[] { "#EB3324", "#4899D0", "#5ABF6C", "#F5F352", "#8B5FF5", "#BC33C7", "#6BE9EB" };// 
        private static Timer? AgentTimer;
        private static Timer? PlayerTimer;
        public static bool ArePlayerPositionsUpdated = false;
        public static Func<Task<Position>>? PositionProvider;
        public static string? GameId { get; set; }
        public static string? PlayerId => HubConnection.ConnectionId;
        public static Player? Player => Players.FirstOrDefault(p => p.ConnectionId == PlayerId);
        public static List<Player> Players { get; } = new();
        public static List<GameIdRecord> GameIdList { get; } = new();

        public delegate void OnChanged();
        public delegate void OnError(string title, string description);
        public static OnChanged? PlayerListObserver;
        public static OnChanged? GameIdListObserver;
        public static OnChanged? StartGameObserver;
        public static OnError? ErrorObserver;

        private static HubConnection? hubConnection;
        public static HubConnection HubConnection
        {
            get
            {
                if (hubConnection == null)
                {
                    hubConnection = new HubConnectionBuilder().WithUrl(App.ServiceHubUrl).Build();
                    hubConnection.On<string, string, string>("ExecuteCommand", ExecuteCommand);
                    // The startAsync is called in Programs, since no async calls are permitted here.
                    // await hubConnection.StartAsync();
                }
                return hubConnection;
            }
        }

        public static void NewGame()
        {
            TerminateGame();
            HubConnection.On<string, string>("ServiceLookup", ServiceLookup);
            GameId = CreateGameId();
            if(HubConnection.ConnectionId != null)            
                Players.Add(new Player("Agent-X", HubConnection.ConnectionId, PlayerColors[Players.Count % PlayerColors.Length], true));
        }

        public static async Task JoinGame(string playerName, string gameId)
        {
            HubConnection.Remove("ServiceLookup");
            GameId = gameId;
            if (HubConnection.ConnectionId != null)
                Players.Add(new Player(playerName, HubConnection.ConnectionId, PlayerColors[Players.Count % PlayerColors.Length]));
            await new JoinGame(playerName).InvokeCommand(HubConnection, gameId);
        }

        // Will be called by Host only
        public static async Task StartGame()
        {
            HubConnection.Remove("ServiceLookup");
            for (int i = 1; i < Game.Players.Count; i++)
            {
                var startGame = new StartGame(i);
                await startGame.InvokeCommand(HubConnection, Players[i].ConnectionId);
            }
            await Task.Delay(100);
            UpdateAgentPosition(null, null);
            UpdatePlayerPositions(null, null);
            AgentTimer = new Timer(TimeSpan.FromSeconds(UpdateIntervalAgent));
            AgentTimer.Elapsed += UpdateAgentPosition;
            AgentTimer.Start();
            PlayerTimer = new Timer(TimeSpan.FromSeconds(Game.UpdateIntervalPlayers));
            PlayerTimer.Elapsed += UpdatePlayerPositions;
            PlayerTimer.Start();
        }

        public static void TerminateGame()
        {
            AgentTimer?.Stop();
            PlayerTimer?.Stop();
            HubConnection.Remove("ServiceLookup");
            Game.Players.Clear();
            Game.GameIdList.Clear();
            GameId = null;
            ArePlayerPositionsUpdated = false;
        }

        private static async void UpdateAgentPosition(Object? source, ElapsedEventArgs? e)
        {
            if (Player == null) return;
            var pos = PositionProvider != null ? (await PositionProvider.Invoke()) : new Position(48.567974421168984, 9.241657912638345);
            Player.Latitude = pos.Latitude;
            Player.Longitude = pos.Longitude;
            Player.NextUpdate = DateTime.Now.AddSeconds(Game.UpdateIntervalAgent);
            var update = new PositionUpdate(Player);
            for (int i = 1; i < Players.Count; i++)
            {
                var player = Players[i];
                await update.InvokeCommand(HubConnection, player.ConnectionId);
            }
            PlayerListObserver?.Invoke();
        }

        private static async void UpdatePlayerPositions(Object? source, ElapsedEventArgs? e)
        {
            if (hubConnection == null) return;
            var positions = new List<Position>();
            for (int i = 1; i < Players.Count; i++)
            {
                var cmd = new PositionRequest();
                try
                {
                    var player = Players[i];
                    var pos = await cmd.InvokeCommand(hubConnection, Players[i].ConnectionId);
                    if (player != null)
                    {
                        player.Longitude = pos.Longitude;
                        player.Latitude = pos.Latitude;
                        player.Color = PlayerColors[i % PlayerColors.Length];
                        player.NextUpdate = DateTime.Now.AddSeconds(UpdateIntervalPlayers);
                    }
                }
                catch (Exception ex)
                {
                    ErrorObserver?.Invoke("Error", $"Could not update position of player {Players[i].Name}.{Environment.NewLine}{ex.Message}");
                }
            }
            ArePlayerPositionsUpdated = true;
            var update = new PositionUpdate(Players.ToArray());
            for (int i = 1; i < Players.Count; i++)
            {
                var player = Players[i];
                await update.InvokeCommand(hubConnection, player.ConnectionId);
            }
            PlayerListObserver?.Invoke();
        }

        private static string CreateGameId()
        {
            var b = new StringWriter();
            var r = new Random();
            for (int i = 0; i < 4; i++) b.Write("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"[r.Next(36)]);
            return b.ToString();
        }

        public static async Task UpdateGameIdList()
        {
            await HubConnection.SendAsync("ServiceLookup", "AgentXSession");
        }

        private static async Task ServiceLookup(string serviceType, string connectionId)
        {
            if ("AgentXSession".Equals(serviceType) && GameId != null)
            {
                await new SubmitGameId(GameId).InvokeCommand(HubConnection, connectionId);
            }
        }

        private static async Task<string> ExecuteCommand(string jsonCmd, string connectionId)
        {
            if (HubConnection != null)
            {
                var anyCmd = CommandUtil.Deserialize(jsonCmd);
                if (anyCmd is JoinGame joinCmd)
                {
                    var existing = Game.Players.FirstOrDefault(x => x.ConnectionId == connectionId);
                    if (existing != null) Game.Players.Remove(existing);
                    Game.Players.Add(new Player(joinCmd.PlayerName, connectionId));
                    PlayerListObserver?.Invoke();
                    return await joinCmd.ReturnResult("Result");
                }
                else if (anyCmd is SubmitGameId GameIdCmd)
                {
                    GameIdList.Add(new GameIdRecord(GameIdCmd.GameId, connectionId));
                    GameIdListObserver?.Invoke();
                    return await GameIdCmd.ReturnResult("Result");
                }
                else if (anyCmd is StartGame startCmd)
                {
                    StartGameObserver?.Invoke();
                    return await startCmd.ReturnResult("Started");
                }
                else if (anyCmd is PositionRequest pr)
                {
                       
                    var pos = PositionProvider != null ? (await PositionProvider.Invoke()) : new Position(48.567974421168984, 9.241657912638345);
                    return await pr.ReturnResult(pos);
                }
                else if (anyCmd is PositionUpdate pu)
                {
                    Players.Clear();
                    Players.AddRange(pu.Players);
                    PlayerListObserver?.Invoke();
                    ArePlayerPositionsUpdated = true;
                    return await pu.ReturnResult("updated");
                }
            }
            return "";
        }
    }
}