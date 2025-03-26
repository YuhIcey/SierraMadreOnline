using System.Net.Http;
using System.Text;
using System.Text.Json;
using MadreServer.Config;

namespace MadreServer.MadreStartup
{
    public static class MadreDiscord
    {
        private static DiscordConfig? _config;
        private static readonly HttpClient Http = new();

        public static void Init()
        {
            _config = DiscordConfig.Load();

            if (!_config.Enabled || string.IsNullOrWhiteSpace(_config.Webhook))
            {
                Console.WriteLine("❌ Discord not active.");
                return;
            }

            Console.WriteLine($"🤖 Discord ready. Webhook: {_config.Webhook}");
        }

        public static void Send(string message)
        {
            if (_config?.Enabled != true || string.IsNullOrWhiteSpace(_config.Webhook)) return;

            var payload = new { content = message };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Http.PostAsync(_config.Webhook, content).Wait();
        }

        public static string GetModRoleId() => _config?.ModRoleId ?? "";
    }
}
