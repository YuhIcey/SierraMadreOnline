using Fleck;
using MadreServer.MadreShyro;
using MadreServer.Madre;
using Newtonsoft.Json;

namespace MadreServer.MadreShyro
{
    public static class ShyroServer
    {
        private static WebSocketServer? _server;
        public static List<IWebSocketConnection> Clients { get; } = new();
        private static Dictionary<IWebSocketConnection, string> ClientToPlayerMap = new();

        public static void Start(int port)
        {
            FleckLog.Level = LogLevel.Warn;
            _server = new WebSocketServer($"ws://0.0.0.0:{port}");

            _server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Clients.Add(socket);
                    Console.WriteLine($"🔌 Client connected: {socket.ConnectionInfo.ClientIpAddress}");
                };

                socket.OnClose = () =>
                {
                    Clients.Remove(socket);

                    if (ClientToPlayerMap.TryGetValue(socket, out var playerId))
                    {
                        PlayerManager.RemovePlayer(playerId);
                        ClientToPlayerMap.Remove(socket);
                        Console.WriteLine($"❌ Player '{playerId}' disconnected and removed from manager.");
                    }
                    else
                    {
                        Console.WriteLine($"❌ Unknown client disconnected.");
                    }
                };

                socket.OnMessage = msg =>
                {
                    try
                    {
                        var json = JsonConvert.DeserializeObject<dynamic>(msg);
                        string type = json?.type;

                        if (type == "position")
                        {
                            var state = JsonConvert.DeserializeObject<PlayerState>(msg);
                            if (state != null && !string.IsNullOrWhiteSpace(state.PlayerId))
                            {
                                ClientToPlayerMap[socket] = state.PlayerId;
                            }
                        }

                        ShyroDispatcher.HandleMessage(socket, msg);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error parsing socket message: {ex.Message}");
                    }
                };
            });

            Console.WriteLine($"✅ Shyro WebSocket Server started on port {port}");
        }
    }
}
