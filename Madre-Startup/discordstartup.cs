using System;
using MadreServer.Config;

namespace MadreServer.MadreStartup
{
    public static class MadreDiscord
    {
        private static DiscordConfig? _config;

        public static void Init()
        {
            _config = DiscordConfig.Load();

            if (!_config.Enabled)
            {
                Console.WriteLine("🛑 Discord integration is disabled.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_config.Webhook))
            {
                Console.WriteLine("⚠️ Discord is enabled but webhook is missing!");
                return;
            }

            Console.WriteLine("🤖 Discord integration enabled.");
            Console.WriteLine($"🌐 Webhook: {_config.Webhook}");
        }

        public static void Send(string message)
        {
            if (_config?.Enabled == true)
                Console.WriteLine($"[Discord] {message}");
        }
    }
}
