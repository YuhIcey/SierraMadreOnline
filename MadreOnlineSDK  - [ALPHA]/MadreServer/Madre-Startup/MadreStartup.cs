using System;
using System.IO;
using System.Threading;
using Fleck;
using MadreServer.Config;


namespace MadreServer
{
    public static class MadreStartup
    {
        private static ServerConfig? _config;
        private static WebSocketServer? _wsServer;

        public static void Start()
        {
            Console.Title = "The Madre Online 🌒";
            Console.WriteLine(">> Loading The Madre...");

            LoadConfig();
            InitWebSocket();
            StartServerLoop();

            Console.WriteLine($"✅ Madre server is online at ws://{_config!.Host}:{_config.Port}");
            Console.WriteLine($"👥 Max Players: {_config.MaxPlayers} | MOTD: {_config.Motd}");
        }

        private static void LoadConfig()
        {
            var configPath = "Config/serverconfig.cfg";

            if (!File.Exists(configPath))
            {
                Console.WriteLine("❌ Config file not found. Creating default...");
                Directory.CreateDirectory("Config");

                File.WriteAllText(configPath,
@"# MadreServer configuration file
host=localhost
port=3333
maxPlayers=64
motd=Welcome to The Madre Online!");

                _config = ServerConfig.LoadFromFile(configPath);
            }
            else
            {
                _config = ServerConfig.LoadFromFile(configPath);
            }
        }

        private static void InitWebSocket()
        {
            _wsServer = new WebSocketServer($"ws://{_config!.Host}:{_config.Port}");
            _wsServer.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine($"🔌 Client connected: {socket.ConnectionInfo.ClientIpAddress}");
                };

                socket.OnClose = () =>
                {
                    Console.WriteLine($"❌ Client disconnected: {socket.ConnectionInfo.ClientIpAddress}");
                };

                socket.OnMessage = msg =>
                {
                    Console.WriteLine($"📡 Received: {msg}");

                    // Later: Deserialize & broadcast to other clients Plz do not edit. 
                };
            });
        }

        private static void StartServerLoop()
        {
            new Thread(() =>
            {
                while (true)
                {
                    // Future tick logic (ping checks, game rules, etc.) - Leave for now
                    Thread.Sleep(100);
                }
            })
            { IsBackground = true }.Start();
        }
    }
}
