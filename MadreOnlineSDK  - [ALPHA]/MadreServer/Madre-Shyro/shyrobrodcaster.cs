using Fleck;

namespace MadreServer.MadreShyro
{
    public static class ShyroBroadcaster
    {
        /// <summary>
        /// Broadcast to ALL connected clients
        /// </summary>
        public static void Broadcast(string message)
        {
            foreach (var client in ShyroServer.Clients)
            {
                if (client.IsAvailable)
                    client.Send(message);
            }
        }

        /// <summary>
        /// Broadcast to all EXCEPT the sender
        /// </summary>
        public static void BroadcastExcept(IWebSocketConnection exclude, string message)
        {
            foreach (var client in ShyroServer.Clients)
            {
                if (client != exclude && client.IsAvailable)
                    client.Send(message);
            }
        }

        /// <summary>
        /// Send a message to a specific client by IP (optional helper)
        /// </summary>
        public static void SendTo(string ipAddress, string message)
        {
            var client = ShyroServer.Clients.FirstOrDefault(c =>
                c.ConnectionInfo.ClientIpAddress == ipAddress && c.IsAvailable);

            client?.Send(message);
        }
    }
}
