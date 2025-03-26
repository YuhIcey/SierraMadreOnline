using System;
using System.Threading;
using MadreServer.Hasher;

namespace MadreServer.Admin
{
    public static class ServerCommandInterface
    {
        private static Thread? _commandThread;
        private static string? _currentAdmin;

        public static void Start()
        {
            Console.WriteLine("🔐 Admin authentication required. Use: login <name> <token>");

            _commandThread = new Thread(() =>
            {
                while (true)
                {
                    Console.Write("> ");
                    var input = Console.ReadLine()?.Trim();

                    if (string.IsNullOrWhiteSpace(input)) continue;

                    var args = input.Split(' ', 3);
                    var cmd = args[0].ToLower();

                    switch (cmd)
                    {
                        case "login":
                            if (args.Length < 3)
                            {
                                Console.WriteLine("Usage: login <name> <token>");
                                break;
                            }

                            var name = args[1];
                            var token = args[2];

                            if (AdminToken.ValidateToken(name, token))
                            {
                                _currentAdmin = name;
                                Console.WriteLine($"✅ Welcome, Admin {name}.");
                            }
                            else
                            {
                                Console.WriteLine("❌ Invalid token.");
                            }
                            break;

                        case "status":
                            if (!IsAuthed()) break;
                            Console.WriteLine("🧠 Server is running. Current admin: " + _currentAdmin);
                            break;

                        case "exit":
                            if (!IsAuthed()) break;
                            Console.WriteLine("👋 Shutting down...");
                            Environment.Exit(0);
                            break;

                        default:
                            Console.WriteLine("❓ Unknown command.");
                            break;
                    }
                }
            });

            _commandThread.Start();
        }

        private static bool IsAuthed()
        {
            if (string.IsNullOrWhiteSpace(_currentAdmin))
            {
                Console.WriteLine("🔒 Access denied. Login required.");
                return false;
            }

            return true;
        }
    }
}
