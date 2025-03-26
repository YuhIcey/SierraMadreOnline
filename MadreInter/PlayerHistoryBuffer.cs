using System.Collections.Generic;
using MadreServer.MadreShared;

namespace MadreServer.MadreNet
{
    public class PlayerHistoryBuffer
    {
        private readonly Queue<PlayerState> _history = new();
        private readonly int _maxTicks = 100;

        public void Record(PlayerState state)
        {
            _history.Enqueue(state);
            while (_history.Count > _maxTicks)
                _history.Dequeue();
        }

        public PlayerState? GetStateAt(long tick)
        {
            foreach (var state in _history)
            {
                if (state.LastUpdateTick == tick)
                    return state;
            }
            return null;
        }

        public IEnumerable<PlayerState> GetAll() => _history;
    }
}
