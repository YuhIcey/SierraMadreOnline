using Fleck;
using MadreServer.MadreShyro;
using MadreServer.Madre;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MadreServer.MadreShyro
{
    public static class ShyroServer
    {
        private static WebSocketServer? _server;
        public static List<IWebSocketConnection> Clients { get; } = new();
        private static Dictionary<IWebSocketConnection, string> ClientToPlayerMap = new();
        // Track a simple sequence number per client.
        private static Dictionary<IWebSocketConnection, int> ClientSequenceNumbers = new();

        public static void Start(int port)
        {
            FleckLog.Level = LogLevel.Warn;
            _server = new WebSocketServer($"ws://0.0.0.0:{port}");

            _server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Clients.Add(socket);
                    ClientSequenceNumbers[socket] = 0;
                    Console.WriteLine($"🔌 Client connected: {socket.ConnectionInfo.ClientIpAddress}");
                };

                socket.OnClose = () =>
                {
                    Clients.Remove(socket);
                    ClientSequenceNumbers.Remove(socket);

                    if (ClientToPlayerMap.TryGetValue(socket, out var playerId))
                    {
                        // Optionally remove the player state from the manager.
                        ShyroPlayerManager.TryGet(playerId, out var _);
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
                        // If the message is a "position" update, store the player's ID.
                        var json = JsonConvert.DeserializeObject<dynamic>(msg);
                        string? type = json?.type;
                        if (type == "position")
                        {
                            var state = JsonConvert.DeserializeObject<PlayerState>(msg);
                            if (state != null && !string.IsNullOrWhiteSpace(state.PlayerId))
                            {
                                ClientToPlayerMap[socket] = state.PlayerId;
                            }
                        }

                        // Process the message.
                        ShyroDispatcher.HandleMessage(socket, msg);

                        // Increment the sequence number for this client.
                        if (ClientSequenceNumbers.ContainsKey(socket))
                        {
                            ClientSequenceNumbers[socket]++;
                        }
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
