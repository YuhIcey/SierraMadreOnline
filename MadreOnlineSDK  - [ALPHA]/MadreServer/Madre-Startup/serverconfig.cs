namespace MadreServer.Config
{
    public class ServerConfig
    {
        public string Host { get; set; } = "localhost";

        // Hardcoded port
        public int Port => 7777;

        public int MaxPlayers { get; set; } = 64;
        public string Motd { get; set; } = "Welcome to The Madre Online!";

        public static ServerConfig LoadFromFile(string path)
        {
            var config = new ServerConfig();

            if (!File.Exists(path))
                return config;

            var lines = File.ReadAllLines(path);

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#")) continue;

                var parts = line.Split('=', 2);
                if (parts.Length != 2) continue;

                var key = parts[0].Trim().ToLower();
                var value = parts[1].Trim();

                switch (key)
                {
                    case "host": config.Host = value; break;
                    case "maxplayers": config.MaxPlayers = int.TryParse(value, out var max) ? max : config.MaxPlayers; break;
                    case "motd": config.Motd = value; break;
                    // Port is ignored on purpose make sure it does not change
                }
            }

            return config;
        }
    }
}
