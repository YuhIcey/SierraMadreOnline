using MadreServer.MadreShyro;
using MadreServer.MadreStartup;
using System.IO;

namespace MadreServer.Madre
{
    public static class Chat
    {
        private const float LocalChatRange = 200f;

        public static void HandleMessage(string playerId, string message)
        {
            if (ShyroPlayerManager.IsMuted(playerId))
            {
                ShyroBroadcaster.SendTo(playerId, "🔇 You are muted.");
                return;
            }

            bool isGlobal = message.StartsWith("//");
            string cleanMessage = isGlobal ? message.Substring(2).TrimStart() : message;

            string formatted = isGlobal
                ? $"🌐 [GLOBAL] {playerId}: {cleanMessage}"
                : $"🗨️ [LOCAL] {playerId}: {cleanMessage}";

            if (!ShyroPlayerManager.TryGet(playerId, out MadreServer.MadreShyro.PlayerState? sender) || sender == null)
                return;

            if (isGlobal)
            {
                ShyroBroadcaster.Broadcast(formatted);
                MadreDiscord.Send($"🌐 **[GLOBAL]** `{playerId}`: {cleanMessage}");
                LogToAdminFile($"{DateTime.Now} [GLOBAL] {playerId}: {cleanMessage}");
                return;
            }

            foreach (var target in ShyroPlayerManager.GetAll())
            {
                if (target.PlayerId == sender.PlayerId) continue;

                if (DistanceSquared(sender, target) <= LocalChatRange * LocalChatRange)
                    ShyroBroadcaster.SendTo(target.PlayerId, formatted);
            }

            ShyroBroadcaster.SendTo(sender.PlayerId, formatted);
            LogToAdminFile($"{DateTime.Now} [LOCAL] {playerId}: {cleanMessage}");
        }

        private static float DistanceSquared(MadreServer.MadreShyro.PlayerState a, MadreServer.MadreShyro.PlayerState b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            float dz = a.Z - b.Z;
            return dx * dx + dy * dy + dz * dz;
        }

        private static void LogToAdminFile(string message)
        {
            Directory.CreateDirectory("Logs");
            File.AppendAllText("Logs/admin_chat.log", message + Environment.NewLine);
        }
    }
}
