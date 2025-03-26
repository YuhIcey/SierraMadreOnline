using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spectre.Console;

namespace MadreServer.MadreSocials
{
    public enum FactionTier
    {
        Group,
        Division,
        Faction
    }

    public enum FactionRank
    {
        Member,
        NCO,
        Officer,
        CoOwner,
        Owner
    }

    public class Faction
    {
        public string Tag { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public FactionTier Tier { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OwnerSteamId { get; set; } = string.Empty;

        public Dictionary<string, FactionRank> MemberRoles { get; set; } = new();
    }

    public static class FactionManager
    {
        private static readonly Dictionary<string, Faction> Factions = new();
        private const string ConfigPath = "Config/factions.cfg";

        public static void Load()
        {
            if (!File.Exists(ConfigPath)) return;

            foreach (var line in File.ReadAllLines(ConfigPath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                var parts = line.Split('|');
                if (parts.Length < 6) continue;

                var tag = parts[0].Trim();
                var name = parts[1].Trim();
                if (!Enum.TryParse(parts[2].Trim(), out FactionTier tier)) continue;
                if (!DateTime.TryParse(parts[3].Trim(), out DateTime created)) continue;
                var owner = parts[4].Trim();
                var rawMembers = parts[5].Trim();

                var memberRoles = new Dictionary<string, FactionRank>();
                foreach (var entry in rawMembers.Split(','))
                {
                    var split = entry.Split(':');
                    if (split.Length == 2 && Enum.TryParse(split[1], out FactionRank rank))
                        memberRoles[split[0]] = rank;
                }

                var faction = new Faction
                {
                    Tag = tag,
                    Name = name,
                    Tier = tier,
                    CreatedAt = created,
                    OwnerSteamId = owner,
                    MemberRoles = memberRoles
                };

                Factions[tag] = faction;
            }
        }

        public static void Save()
        {
            var lines = new List<string>
            {
                "# Tag | Name | Tier | CreatedAt | OwnerSteamID | Members"
            };

            lines.AddRange(Factions.Values.Select(f =>
            {
                var members = string.Join(",", f.MemberRoles.Select(r => $"{r.Key}:{r.Value}"));
                return $"{f.Tag} | {f.Name} | {f.Tier} | {f.CreatedAt:O} | {f.OwnerSteamId} | {members}";
            }));

            Directory.CreateDirectory("Config");
            File.WriteAllLines(ConfigPath, lines);
        }

        public static bool CreateFaction(string name, string creatorSteamId, out string tag)
        {
            tag = "";

            if (Factions.Values.Any(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return false;

            tag = GenerateUniqueTag(name);
            var faction = new Faction
            {
                Name = name,
                Tag = tag,
                Tier = FactionTier.Group,
                CreatedAt = DateTime.UtcNow,
                OwnerSteamId = creatorSteamId,
                MemberRoles = new Dictionary<string, FactionRank>
                {
                    { creatorSteamId, FactionRank.Owner }
                }
            };

            Factions[tag] = faction;
            Save();
            return true;
        }

        public static bool ChangeFactionOwner(string tag, string newSteamId)
        {
            if (!Factions.TryGetValue(tag, out var faction)) return false;
            faction.OwnerSteamId = newSteamId;
            if (faction.MemberRoles.ContainsKey(newSteamId))
                faction.MemberRoles[newSteamId] = FactionRank.Owner;
            else
                faction.MemberRoles.Add(newSteamId, FactionRank.Owner);

            Save();
            return true;
        }

        public static bool SetMemberRole(string tag, string targetId, FactionRank newRank, string actorId)
        {
            if (!Factions.TryGetValue(tag, out var faction)) return false;
            if (!faction.MemberRoles.ContainsKey(targetId)) return false;
            if (!HasPermission(tag, actorId, FactionRank.Officer)) return false;

            faction.MemberRoles[targetId] = newRank;
            Save();
            return true;
        }

        public static bool HasPermission(string tag, string steamId, FactionRank required)
        {
            if (!Factions.TryGetValue(tag, out var faction)) return false;
            if (!faction.MemberRoles.TryGetValue(steamId, out var actual)) return false;
            return actual >= required;
        }

        private static string GenerateUniqueTag(string name)
        {
            var baseTag = new string(name.ToUpper().Where(char.IsLetter).Take(3).ToArray());
            if (baseTag.Length < 3) baseTag = baseTag.PadRight(3, 'X');

            var tag = baseTag;
            int i = 1;
            while (Factions.ContainsKey(tag))
            {
                tag = $"{baseTag[0]}{baseTag[1]}{(char)('A' + i)}";
                i++;
            }

            return tag;
        }

        public static void PromoteEligibleFactions()
        {
            foreach (var faction in Factions.Values)
            {
                if (faction.Tier == FactionTier.Group &&
                    (DateTime.UtcNow - faction.CreatedAt).TotalHours >= 120)
                {
                    faction.Tier = FactionTier.Division;
                    AnsiConsole.MarkupLine($"[green]⏫ {faction.Name} has been promoted to Division![/]");
                }
            }

            Save();
        }

        public static bool ApproveToFaction(string tag)
        {
            if (!Factions.TryGetValue(tag, out var faction)) return false;
            if (faction.Tier != FactionTier.Division) return false;

            faction.Tier = FactionTier.Faction;
            Save();
            return true;
        }

        public static Faction? GetFactionByPlayer(string playerId) =>
            Factions.Values.FirstOrDefault(f => f.MemberRoles.ContainsKey(playerId));

        public static Faction? GetFactionByTag(string tag) =>
            Factions.TryGetValue(tag, out var faction) ? faction : null;

        public static IEnumerable<Faction> GetAll() => Factions.Values;
    }
}
