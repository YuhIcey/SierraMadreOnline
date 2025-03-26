namespace MadreServer.MadreSocials
{
    public class CapturePoint
    {
        public string Id { get; set; } = string.Empty;    // initialized
        public string Name { get; set; } = string.Empty;  // initialized

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Radius { get; set; }

        public string? ControllingFaction { get; set; }

        public Dictionary<string, int> PresenceCount { get; set; } = new();

        public bool IsInside(float x, float y, float z)
        {
            float dx = x - X;
            float dy = y - Y;
            float dz = z - Z;
            return dx * dx + dy * dy + dz * dz <= Radius * Radius;
        }
    }
}
