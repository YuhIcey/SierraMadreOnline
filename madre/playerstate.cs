namespace MadreServer.Models
{
    public class PlayerState
    {
        public string PlayerId { get; set; } = string.Empty;
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
    }
}
