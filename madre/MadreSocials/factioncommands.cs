using System;
using System.Linq;
using MadreServer.MadreShyro;
using MadreServer.MadreSocials;

namespace MadreServer.MadreSocials
{
    public static class FactionCommands
    {
        public static void Handle(string playerId, string[] args)
        {
            if (args.Length == 0)
            {
                ShyroBroadcaster.SendTo(playerId, "❌ Usage: /f <create|invite|promote|demote|chat>");
                return;
            }

            var cmd = args[0].ToLower();

            switch (cmd)
            {
                case "create":
                    if (args.Length < 2)
                    {
                        ShyroBroadcaster.SendTo(playerId, "❌ Usage: /f create <name>");
                        return;
                    }

                    var name = string.Join(" ", args.Skip(1));
                    if (FactionManager.CreateFaction(name, playerId, out var tag))
                    {
                        ShyroBroadcaster.SendTo(playerId, $"✅ Faction '{name}' [{tag}] created.");
                    }
                    else
                    {
                        ShyroBroadcaster.SendTo(playerId, "❌ Faction name already taken.");
                    }
                    break;

                case "chat":
                    var chatMsg = string.Join(" ", args.Skip(1));
                    var faction = FactionManager.GetFactionByPlayer(playerId);
                    if (faction == null)
                    {
                        ShyroBroadcaster.SendTo(playerId, "❌ You are not in a faction.");
                        return;
                    }

                    var msg = $"🏳️ [{faction.Tag}] {playerId}: {chatMsg}";
                    foreach (var member in faction.MemberRoles.Keys)
                        ShyroBroadcaster.SendTo(member, msg);
                    break;

                default:
                    ShyroBroadcaster.SendTo(playerId, "❌ Unknown faction command.");
                    break;
            }
        }
    }
}
