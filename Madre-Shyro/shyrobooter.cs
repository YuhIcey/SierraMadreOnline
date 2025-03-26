using System;
using MadreServer.Config;

namespace MadreServer.MadreShyro
{
    public static class ShyroBooter
    {
        public static void Init(ServerConfig config)
        {
            Console.WriteLine("🔧 Initializing Shyro Network Layer...");

            // Start the server on the configured port.
            ShyroServer.Start(config.Port);

            // Start tick processing (e.g. for pings, reliability, etc.)
            ShyroTick.Start();

            Console.WriteLine("✅ Shyro is live and ticking.");
        }
    }
}
