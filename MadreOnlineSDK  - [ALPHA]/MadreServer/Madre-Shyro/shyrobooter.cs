using System;
using MadreServer.Config;

namespace MadreServer.MadreShyro
{
    public static class ShyroBooter
    {
        public static void Init(ServerConfig config)
        {
            Console.WriteLine("🌐 Initializing Shyro Network Layer...");

            // Start WebSocket server
            ShyroServer.Start(config);

            // Start server-side tick loop
            ShyroTick.Start();

            Console.WriteLine("✅ Shyro is live and ticking.");
        }
    }
}
