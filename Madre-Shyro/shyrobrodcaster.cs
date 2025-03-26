using Fleck;
using System.Linq;

namespace MadreServer.MadreShyro
{
    public static class ShyroBroadcaster
    {
        // Broadcast a message to ALL connected clients.
        public static void Broadcast(string message)
        {
            foreach (var client in ShyroServer.Clients)
            {
                if (client.IsAvailable)
                    client.Send(message);
            }
        }

        // Broadcast to all EXCEPT the sender.
        public static void BroadcastExcept(IWebSocketConnection exclude, string message)
        {
            foreach (var client in ShyroServer.Clients)
            {
                if (client != exclude && client.IsAvailable)
                    client.Send(message);
            }
        }

        // Send a message to a specific client by IP.
        public static void SendTo(string ipAddress, string message)
        {
            var client = ShyroServer.Clients.FirstOrDefault(c =>
                c.ConnectionInfo.ClientIpAddress == ipAddress && c.IsAvailable);

            client?.Send(message);
        }
    }
}
