using Fleck;
using Newtonsoft.Json;
using MadreServer.Models;
using MadreServer.Madre;
using MadreServer.MadreShyro;

namespace MadreServer.MadreShyro
{
    public static class ShyroDispatcher
    {
        public static void HandleMessage(IWebSocketConnection socket, string msg)
        {
            try
            {
                var json = JsonConvert.DeserializeObject<dynamic>(msg);
                string type = json?.type;

                switch (type)
                {
                    case "ping":
                        socket.Send("{\"type\":\"pong\"}");
                        break;

                    case "position":
                        var state = JsonConvert.DeserializeObject<PlayerState>(msg);
                        if (state == null || string.IsNullOrWhiteSpace(state.PlayerId))
                        {
                            Console.WriteLine("⚠️ Malformed player position data.");
                            break;
                        }

                        PlayerManager.UpdatePlayer(state);

                        // Broadcast to all other players except the sender
                        var updateJson = JsonConvert.SerializeObject(state);
                        ShyroBroadcaster.BroadcastExcept(socket, updateJson);

                        Console.WriteLine($"📍 {state.PlayerId} => {state.X}, {state.Y}, {state.Z}");
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
    }
}
