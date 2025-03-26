using System;
using System.Collections.Generic;
using System.IO;

namespace MadreServer.Admin
{
    public static class AdminRegistry
    {
        private static readonly Dictionary<string, AdminRecord> Admins = new();

        public static void LoadFromCfg(string path = "Config/admin.cfg")
        {
            if (!File.Exists(path))
            {
                Directory.CreateDirectory("Config");
                File.WriteAllText(path,
@"# Format: steamid64,role
76561198000000001,Owner
76561198000000002,Admin
76561198000000003,Mod");
            }

            foreach (var line in File.ReadAllLines(path))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                var parts = line.Split(',');
                if (parts.Length != 2) continue;

                var steamId = parts[0].Trim();
                if (!Enum.TryParse(parts[1].Trim(), out AdminRole role)) continue;

                Admins[steamId] = new AdminRecord(steamId, role);
            }
        }

        public static bool IsAdmin(string steamId) => Admins.ContainsKey(steamId);

        public static AdminRole? GetRole(string steamId) =>
            Admins.TryGetValue(steamId, out var record) ? record.Role : null;

        public static IEnumerable<AdminRecord> GetAll() => Admins.Values;
    }

    public class AdminRecord
    {
        public string SteamId { get; }
        public AdminRole Role { get; }

        public AdminRecord(string steamId, AdminRole role)
        {
            SteamId = steamId;
            Role = role;
        }
    }
}
