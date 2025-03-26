using System.Collections.Generic;
using MadreServer.MadreZones;
using MadreServer.MadreInterop;
using MadreServer.MadreShyro;

namespace MadreServer.MadrePlayers
{
    public static class PlayerStatusManager
    {
        private static readonly HashSet<string> PvPEnabled = new();
        private static readonly HashSet<string> InSafeZone = new();

        public static void UpdateZoneStatus(string playerId, float x, float y, float z)
        {
            bool isInSafe = SafeZoneManager.IsInSafeZone(x, y, z);

            if (isInSafe && !InSafeZone.Contains(playerId))
            {
                InSafeZone.Add(playerId);
                EnableGodMode(playerId);
                EnableInfiniteAmmo(playerId);
                ShyroBroadcaster.SendTo(playerId, "🟢 You’ve entered a Safe Zone. Godmode & Ammo granted.");
            }
            else if (!isInSafe && InSafeZone.Contains(playerId))
            {
                InSafeZone.Remove(playerId);
                DisableGodMode(playerId);
                DisableInfiniteAmmo(playerId);
                ShyroBroadcaster.SendTo(playerId, "🔴 You’ve left the Safe Zone. PvP now possible.");
            }
        }

        public static void TogglePvP(string playerId)
        {
            if (PvPEnabled.Contains(playerId))
            {
                PvPEnabled.Remove(playerId);
                ShyroBroadcaster.SendTo(playerId, "🔒 PvP Disabled. You cannot be attacked or attack.");
            }
            else
            {
                PvPEnabled.Add(playerId);
                ShyroBroadcaster.SendTo(playerId, "⚔️ PvP Enabled. You can now engage with others who opted in.");
            }
        }

        public static bool IsPvPEnabled(string playerId) => PvPEnabled.Contains(playerId);

        public static bool CanEngagePvP(string a, string b) =>
            PvPEnabled.Contains(a) && PvPEnabled.Contains(b);

        private static void EnableGodMode(string playerId)
        {
            NativeHooks.SetGodMode(playerId, true);
        }

        private static void DisableGodMode(string playerId)
        {
            NativeHooks.SetGodMode(playerId, false);
        }

        private static void EnableInfiniteAmmo(string playerId)
        {
            NativeHooks.SetInfiniteAmmo(playerId, true);
        }

        private static void DisableInfiniteAmmo(string playerId)
        {
            NativeHooks.SetInfiniteAmmo(playerId, false);
        }
    }
}
