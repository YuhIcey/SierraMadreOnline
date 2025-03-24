using System;
using System.Threading;

namespace MadreServer.MadreShyro
{
    public static class ShyroTick
    {
        private static Thread? _tickThread;
        private static bool _running = false;
        private static int _tickInterval = 100; // ms

        public static void Start()
        {
            if (_running) return;

            _running = true;
            _tickThread = new Thread(() =>
            {
                while (_running)
                {
                    Tick();
                    Thread.Sleep(_tickInterval);
                }
            });

            _tickThread.IsBackground = true;
            _tickThread.Start();

            Console.WriteLine("⏱️ ShyroTick started at " + _tickInterval + "ms interval");
        }

        public static void Stop()
        {
            _running = false;
            Console.WriteLine("🛑 ShyroTick stopped.");
        }

        private static void Tick()
        {
            // For now, just log tick count or time
            Console.WriteLine($"[TICK] {DateTime.UtcNow:HH:mm:ss.fff}");

            // Later: Ping checks, event triggers, state broadcasting, etc.
        }
    }
}
