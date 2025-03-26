using System.Collections.Concurrent;
using MadreServer.Madre;
using Fleck; // Required for kick socket close

namespace MadreServer.MadreShyro
{
    public static class ShyroPlayerManager
    {
        private static readonly ConcurrentDictionary<string, PlayerState> Players = new();
        private static readonly HashSet<string> Banned = new();
        private static readonly HashSet<string> Muted = new();
        private static readonly ConcurrentDictionary<string, bool> AdminFlags = new();
        private static readonly ConcurrentDictionary<string, IWebSocketConnection> SocketMap = new();

        public static void UpdatePlayer(string steamId, float x, float y, float z, float rotation)
        {
            if (IsBanned(steamId)) return;

            var state = new PlayerState
            {
                PlayerId = steamId,
                X = x,
                Y = y,
                Z = z,
                Yaw = rotation,
                LastUpdateTick = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            Players[steamId] = state;
        }

        public static IEnumerable<PlayerState> GetAll() => Players.Values;

        public static bool TryGet(string steamId, out PlayerState? state) =>
            Players.TryGetValue(steamId, out state);

        public static void RegisterSocket(string playerId, IWebSocketConnection socket)
        {
            SocketMap[playerId] = socket;
        }

        public static void Kick(string playerId, string reason = "No reason provided")
        {
            if (Players.TryRemove(playerId, out _))
            {
                if (SocketMap.TryRemove(playerId, out var socket))
                {
                    socket.Send($"{{\"type\":\"kick\",\"reason\":\"{reason}\"}}");
                    socket.Close();
                }

                Console.WriteLine($"❌ {playerId} was kicked. Reason: {reason}");
            }
        }

        public static void Ban(string playerId)
        {
            Banned.Add(playerId);
            Kick(playerId, "You have been banned.");
            Console.WriteLine($"⛔ {playerId} was banned.");
        }

        public static void Mute(string playerId)
        {
            Muted.Add(playerId);
            Console.WriteLine($"🔇 {playerId} was muted.");
        }

        public static void SetAdmin(string playerId, bool isAdmin = true)
        {
            AdminFlags[playerId] = isAdmin;
        }

        public static bool IsAdmin(string playerId) =>
            AdminFlags.TryGetValue(playerId, out var isAdmin) && isAdmin;

        public static bool IsMuted(string playerId) => Muted.Contains(playerId);
        public static bool IsBanned(string playerId) => Banned.Contains(playerId);
    }
}
