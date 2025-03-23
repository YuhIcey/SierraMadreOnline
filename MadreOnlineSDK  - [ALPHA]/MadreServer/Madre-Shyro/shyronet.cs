using Fleck;
using MadreServer.Config;

namespace MadreServer.MadreShyro
{
    public static class ShyroServer
    {
        private static WebSocketServer? _server;
        public static List<IWebSocketConnection> Clients { get; } = new();

        public static void Start(ServerConfig config)
        {
            _server = new WebSocketServer($"ws://{config.Host}:{config.Port}");

            _server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine($"🔌 Client connected: {socket.ConnectionInfo.ClientIpAddress}");
                    Clients.Add(socket);
                };

                socket.OnClose = () =>
                {
                    Console.WriteLine($"❌ Client disconnected: {socket.ConnectionInfo.ClientIpAddress}");
                    Clients.Remove(socket);
                };

                socket.OnMessage = msg =>
                {
                    ShyroDispatcher.HandleMessage(socket, msg);
                };
            });
        }
    }
}
