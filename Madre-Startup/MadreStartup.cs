using System;
using System.IO;
using System.Runtime.Versioning;
using System.Threading;
using Fleck;
using MadreServer.Config;
using MadreServer.Hasher;
using MadreServer.Madre;
using MadreServer.Admin;
using Spectre.Console;

namespace MadreServer.Startup
{
    [SupportedOSPlatform("windows")]
    public static class MadreStartup
    {
        private static ServerConfig? _config;
        private static WebSocketServer? _wsServer;

        public static void Start()
        {
            Console.Title = "🌒 The Madre Online";
            ConsoleInterface.Start(); // Spectre-powered CLI

            AnsiConsole.MarkupLine("[blue]>> Loading The Madre...[/]");

            // Validate Fallout NV installation
            string? gamePath = GameLocator.FindFalloutNewVegasPath(true);
            if (string.IsNullOrEmpty(gamePath))
            {
                AnsiConsole.MarkupLine("[red]❌ Fallout NV not found. Server startup aborted.[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[green]✅ Fallout NV found at:[/] [yellow]{gamePath}[/]");

            LoadConfig();
            LoadAdminRoles();
            InitAdminTokens();
            InitWebSocket();
            StartServerLoop();

            AnsiConsole.MarkupLine($"[green]✅ Server Ready:[/] [cyan]ws://{_config!.Host}:{_config.Port}[/]");
            AnsiConsole.MarkupLine($"👥 Max Players: {_config.MaxPlayers} | [grey]{_config.Motd}[/]");
        }

        private static void LoadConfig()
        {
            var configPath = "Config/serverconfig.cfg";

            if (!File.Exists(configPath))
            {
                AnsiConsole.MarkupLine("[red]❌ Config not found. Creating default...[/]");
                Directory.CreateDirectory("Config");

                File.WriteAllText(configPath,
@"# MadreServer configuration
host=localhost
port=7777
maxPlayers=64
motd=Welcome to The Madre Online!
admins=Admin1,Admin2");
            }

            _config = ServerConfig.LoadFromFile(configPath);
        }

        private static void LoadAdminRoles()
        {
            AnsiConsole.MarkupLine("[blue]🔐 Loading admin roles from admin.cfg...[/]");
            AdminRegistry.LoadFromCfg();
            foreach (var admin in AdminRegistry.GetAll())
            {
                AnsiConsole.MarkupLine($"[yellow]👤 {admin.SteamId}[/] → [green]{admin.Role}[/]");
            }
        }

        private static void InitAdminTokens()
        {
            AnsiConsole.MarkupLine("[blue]🔐 Generating tokens for legacy CLI login...[/]");
            foreach (var admin in _config!.Admins)
            {
                var token = AdminToken.GenerateToken(admin);
                AnsiConsole.MarkupLine($"[yellow]🔑 Admin: {admin}[/] → [green]{token}[/]");
            }
        }

        private static void InitWebSocket()
        {
            _wsServer = new WebSocketServer($"ws://{_config!.Host}:{_config.Port}");
            _wsServer.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    AnsiConsole.MarkupLine($"[green]🔌 Client connected:[/] [gray]{socket.ConnectionInfo.ClientIpAddress}[/]");
                };

                socket.OnClose = () =>
                {
                    AnsiConsole.MarkupLine($"[red]❌ Disconnected:[/] [gray]{socket.ConnectionInfo.ClientIpAddress}[/]");
                };

                socket.OnMessage = msg =>
                {
                    AnsiConsole.MarkupLine($"[cyan]📡 Message:[/] {msg}");
                    // Hook into ShyroDispatcher here
                };
            });
        }

        private static void StartServerLoop()
        {
            new Thread(() =>
            {
                while (true)
                {
                    // Tick logic here (e.g., pings, zones, world state)
                    Thread.Sleep(100);
                }
            })
            { IsBackground = true }.Start();
        }
    }
}
