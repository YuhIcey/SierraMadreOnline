using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MadreServer.Config
{
    public class ServerConfig
    {
        public string Host { get; set; } = "localhost";
        public int Port => 7777; // Hardcoded port
        public int MaxPlayers { get; set; } = 64;
        public string Motd { get; set; } = "Welcome to The Madre Online!";
        public List<string> Admins { get; set; } = new();

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
                    case "host":
                        config.Host = value;
                        break;
                    case "maxplayers":
                        config.MaxPlayers = int.TryParse(value, out var max) ? max : config.MaxPlayers;
                        break;
                    case "motd":
                        config.Motd = value;
                        break;
                    case "admins":
                        config.Admins = value.Split(',')
                                             .Select(s => s.Trim())
                                             .Where(s => !string.IsNullOrEmpty(s))
                                             .ToList();
                        break;
                    // Port is intentionally not configurable
                }
            }

            return config;
        }
    }
}
