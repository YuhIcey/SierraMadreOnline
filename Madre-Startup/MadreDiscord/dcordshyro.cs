namespace MadreServer.Config
{
    public class DiscordConfig
    {
        public bool Enabled { get; set; } = false;
        public string Webhook { get; set; } = string.Empty;
        public string ModRoleId { get; set; } = string.Empty;

        public static DiscordConfig Load(string path = "Config/dcord.cfg")
        {
            var config = new DiscordConfig();

            if (!File.Exists(path))
                return config;

            foreach (var rawLine in File.ReadAllLines(path))
            {
                var line = rawLine.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;

                var parts = line.Split('=', 2);
                if (parts.Length != 2) continue;

                var key = parts[0].Trim().ToLower();
                var value = parts[1].Trim();

                switch (key)
                {
                    case "enabled":
                        config.Enabled = bool.TryParse(value, out var enabled) && enabled;
                        break;
                    case "webhook":
                        config.Webhook = value;
                        break;
                    case "modroleid":
                        config.ModRoleId = value;
                        break;
                }
            }

            return config;
        }
    }
}
