using System.Collections.Generic;
using System.Linq;

namespace MadreServer.MadreZones
{
    public static class SafeZoneManager
    {
        private static readonly List<(string Id, float X, float Y, float Z, float Radius)> Zones = new()
        {
            ("goodsprings", 2765.2f, -1230.5f, 135.0f, 75f)
        };

        public static bool IsInSafeZone(float x, float y, float z)
        {
            return Zones.Any(zone =>
            {
                float dx = x - zone.X;
                float dy = y - zone.Y;
                float dz = z - zone.Z;
                return dx * dx + dy * dy + dz * dz <= zone.Radius * zone.Radius;
            });
        }

        public static string? GetZoneName(float x, float y, float z)
        {
            return Zones.FirstOrDefault(zone =>
            {
                float dx = x - zone.X;
                float dy = y - zone.Y;
                float dz = z - zone.Z;
                return dx * dx + dy * dy + dz * dz <= zone.Radius * zone.Radius;
            }).Id;
        }
    }
}
