using System;
using System.Threading;

namespace MadreServer.MadreShyro
{
    public static class ShyroTick
    {
        private static Thread? _tickThread;
        private static bool _running = false;
        private static int _tickInterval = 100;

        public static void Start()
        {
            if (_running) return;

            _running = true;
            _tickThread = new Thread(() =>
            {
                while (_running)
                {
                    ShyroDispatcher.TickAll();
                    Thread.Sleep(_tickInterval);
                }
            });

            _tickThread.IsBackground = true;
            _tickThread.Start();

            Console.WriteLine("⏱️ ShyroTick started.");
        }

        public static void Stop()
        {
            _running = false;
            Console.WriteLine("🛑 ShyroTick stopped.");
        }
    }
}
