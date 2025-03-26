using System;
using System.Linq;
using System.Threading;
using MadreServer.MadreShyro;
using MadreServer.Madre;
using MadreServer.Hasher;
using Spectre.Console;

namespace MadreServer.Startup
{
    public static class ConsoleInterface
    {
        private static string? _currentAdmin;

        public static void Start()
        {
            Console.Title = "🌒 The Madre Online Console";
            PrintBanner();

            new Thread(() =>
            {
                while (true)
                {
                    AnsiConsole.Markup("[blue]> [/]");
                    string? input = Console.ReadLine()?.Trim();
                    if (string.IsNullOrWhiteSpace(input)) continue;

                    var args = input.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
                    var cmd = args[0].ToLower();

                    switch (cmd)
                    {
                        case "login":
                            if (args.Length < 3)
                            {
                                AnsiConsole.MarkupLine("[red]Usage:[/] login <name> <token>");
                                break;
                            }

                            string name = args[1], token = args[2];
                            if (AdminToken.ValidateToken(name, token))
                            {
                                _currentAdmin = name;
                                AnsiConsole.MarkupLine($"[green]✅ Welcome, Admin {name}![/]");
                            }
                            else AnsiConsole.MarkupLine("[red]❌ Invalid token.[/]");
                            break;

                        case "status":
                            RequireAuth(() =>
                            {
                                int count = ShyroPlayerManager.GetAll().Count();
                                AnsiConsole.MarkupLine($"[green]🧠 Server running[/] — [yellow]{count}[/] players connected.");
                            });
                            break;

                        case "kick":
                            RequireAuth(() =>
                            {
                                if (args.Length < 2)
                                {
                                    AnsiConsole.MarkupLine("[red]Usage:[/] kick <playerId>");
                                    return;
                                }
                                ShyroPlayerManager.Kick(args[1]);
                                AnsiConsole.MarkupLine($"[yellow]Player {args[1]} kicked.[/]");
                            });
                            break;

                        case "ban":
                            RequireAuth(() =>
                            {
                                if (args.Length < 2)
                                {
                                    AnsiConsole.MarkupLine("[red]Usage:[/] ban <playerId>");
                                    return;
                                }
                                ShyroPlayerManager.Ban(args[1]);
                                AnsiConsole.MarkupLine($"[red]⛔ {args[1]} banned.[/]");
                            });
                            break;

                        case "players":
                            RequireAuth(() =>
                            {
                                var players = ShyroPlayerManager.GetAll();
                                var table = new Table().AddColumns("ID", "X", "Y", "Z");
                                foreach (var p in players)
                                    table.AddRow(p.PlayerId, p.X.ToString("F2"), p.Y.ToString("F2"), p.Z.ToString("F2"));
                                AnsiConsole.Write(table);
                            });
                            break;

                        case "exit":
                            AnsiConsole.MarkupLine("[blue]👋 Server shutting down...[/]");
                            Environment.Exit(0);
                            break;

                        case "help":
                            ShowHelp();
                            break;

                        default:
                            AnsiConsole.MarkupLine("[grey]Unknown command. Try 'help'.[/]");
                            break;
                    }
                }
            })
            { IsBackground = true }.Start();
        }

        private static void PrintBanner()
        {
            AnsiConsole.Write(new FigletText("Madre Online").Centered().Color(Color.Cyan1));
            AnsiConsole.MarkupLine("[grey]Fallout: New Vegas Multiplayer Server[/]");
            AnsiConsole.MarkupLine("[bold yellow]Type 'login <admin> <token>' to begin[/]\n");
        }

        private static void RequireAuth(Action action)
        {
            if (_currentAdmin == null)
                AnsiConsole.MarkupLine("[red]❌ You must be logged in to use this command.[/]");
            else
                action.Invoke();
        }

        private static void ShowHelp()
        {
            var table = new Table()
                .AddColumns("Command", "Description")
                .AddRow("login <name> <token>", "Authenticate as admin")
                .AddRow("status", "Server summary")
                .AddRow("kick <id>", "Kick player")
                .AddRow("ban <id>", "Ban player")
                .AddRow("players", "List all players")
                .AddRow("help", "Show command list")
                .AddRow("exit", "Shutdown server");
            AnsiConsole.Write(table);
        }
    }
}
