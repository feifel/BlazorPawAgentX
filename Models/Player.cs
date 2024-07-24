using System.Drawing;

namespace BlazorPawAgentX.Models
{
    public class Player(string name, string connectionId, string color = "#4899D0", bool isHost = false, double latitude = 0, double longitude = 0, DateTimeOffset nextUpdate = default)
    {
        public string Name { get; } = name;
        public string ConnectionId { get; } = connectionId;
        public string Color { get; set; } = color;
        public bool IsHost { get; } = isHost;
        public double Latitude { get; set; } = latitude;
        public double Longitude { get; set; } = longitude;
        public DateTimeOffset NextUpdate { get; set;  } = nextUpdate;
    }
}
