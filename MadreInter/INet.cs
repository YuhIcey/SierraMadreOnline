using System.Collections.Generic;
using MadreServer.MadreShared;

namespace MadreServer.MadreNet
{
    public interface INetPlugin
    {
        void RegisterPlayer(string playerId);
        void RemovePlayer(string playerId);
        void Tick();
        void ReceiveState(string playerId, PlayerState state);
        IReadOnlyDictionary<string, INetPlayer> GetAllPlayers();

        // ✅ NEWLY ADDED for CombatHandler support
        INetPlayer? GetPlayer(string id);
    }

    public class NetPlugin : INetPlugin
    {
        private readonly Dictionary<string, INetPlayer> _players = new();
        public static NetPlugin Instance { get; } = new NetPlugin();

        public void RegisterPlayer(string playerId)
        {
            if (!_players.ContainsKey(playerId))
                _players[playerId] = new NetPlayer(playerId);
        }

        public void RemovePlayer(string playerId)
        {
            _players.Remove(playerId);
        }

        public void Tick()
        {
            // Optional logic per-tick
        }

        public void ReceiveState(string playerId, PlayerState state)
        {
            if (!_players.TryGetValue(playerId, out var player))
                RegisterPlayer(playerId);

            _players[playerId].UpdateFromNetwork(state);
        }

        public INetPlayer? GetPlayer(string id)
        {
            return _players.TryGetValue(id, out var p) ? p : null;
        }

        public IReadOnlyDictionary<string, INetPlayer> GetAllPlayers() => _players;
    }
}
