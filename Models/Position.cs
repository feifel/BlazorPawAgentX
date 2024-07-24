namespace BlazorPawAgentX.Models
{
    public class Position(double latitude, double longitude) {
        public double Longitude { get; set; } = longitude;
        public double Latitude { get; set;  } = latitude;
    }
}
