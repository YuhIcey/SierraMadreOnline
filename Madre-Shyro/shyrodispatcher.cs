using Fleck;
using Newtonsoft.Json.Linq;
using MadreServer.Madre;
using MadreServer.MadreNet;
using MadreServer.MadreShared;
using MadreServer.MadreShyro; // Only needed for PlayerManager / Broadcaster

namespace MadreServer.MadreShyro
{
    public static class ShyroDispatcher
    {
        private static readonly INetPlugin NetPlugin = new NetPlugin();

        public static void HandleMessage(IWebSocketConnection socket, string msg)
        {
            try
            {
                var json = JObject.Parse(msg);
                string? type = json["type"]?.ToString();
                int seq = json["seq"]?.Value<int>() ?? -1;

                switch (type)
                {
                    case "ping":
                        var pong = new JObject { ["type"] = "pong", ["seq"] = seq };
                        socket.Send(pong.ToString());
                        break;

                    case "position":
                        var state = json.ToObject<MadreServer.MadreShared.PlayerState>();
                        if (state == null || string.IsNullOrWhiteSpace(state.PlayerId)) break;

                        NetPlugin.ReceiveState(state.PlayerId, state);

                        // Optionally update ShyroPlayerManager for fast lookups
                        ShyroPlayerManager.UpdatePlayer(state.PlayerId, state.X, state.Y, state.Z, state.Yaw);

                        var updateMsg = JObject.FromObject(state);
                        updateMsg["seq"] = seq;
                        ShyroBroadcaster.BroadcastExcept(socket, updateMsg.ToString());
                        break;

                    default:
                        Console.WriteLine($"❓ Unknown message type: {type}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error parsing message: {ex.Message}");
            }
        }

        public static void TickAll()
        {
            NetPlugin.Tick();
        }
    }
}
