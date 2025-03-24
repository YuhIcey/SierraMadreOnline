using System;
using System.Collections.Concurrent;
using MadreServer.Models;

namespace MadreServer.Madre
{
    public static class PlayerManager
    {
        private static readonly ConcurrentDictionary<string, PlayerState> Players = new();

        public static void UpdatePlayer(PlayerState state)
        {
            state.LastUpdate = DateTime.UtcNow;
            Players[state.PlayerId] = state;
        }

        public static PlayerState? GetPlayer(string playerId)
        {
            Players.TryGetValue(playerId, out var state);
            return state;
        }

        public static IEnumerable<PlayerState> GetAllPlayers()
        {
            return Players.Values;
        }

        public static void RemovePlayer(string playerId)
        {
            Players.TryRemove(playerId, out _);
        }

        public static int Count => Players.Count;
    }
}
