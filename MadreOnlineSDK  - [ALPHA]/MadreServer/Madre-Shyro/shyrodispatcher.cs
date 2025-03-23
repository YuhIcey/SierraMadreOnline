using Fleck;
using Newtonsoft.Json;
using MadreServer.Models;

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
                        var playerState = JsonConvert.DeserializeObject<PlayerState>(msg);
                        Console.WriteLine($"📍 Position: {playerState?.PlayerId} => {playerState?.X}, {playerState?.Y}, {playerState?.Z}");

                        if (playerState != null)
                            ShyroBroadcaster.BroadcastExcept(socket, JsonConvert.SerializeObject(playerState));

                        break;

                    default:
                        Console.WriteLine($"⚠️ Unknown message type: {type}");
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
