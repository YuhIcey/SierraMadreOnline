using MadreServer.MadreShared;

namespace MadreServer.MadreNet
{
    public interface INetPlayer
    {
        string PlayerId { get; }
        float X { get; set; }
        float Y { get; set; }
        float Z { get; set; }
        float Yaw { get; set; }
        int Health { get; set; }

        void UpdateFromNetwork(PlayerState state);
        PlayerState ExportState();
        PlayerState? GetStateAtTick(long tick);
    }

    public class NetPlayer : INetPlayer
    {
        public string PlayerId { get; private set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Yaw { get; set; }
        public int Health { get; set; }

        private readonly PlayerHistoryBuffer _history = new();

        public NetPlayer(string id)
        {
            PlayerId = id;
        }

        public void UpdateFromNetwork(PlayerState state)
        {
            X = state.X;
            Y = state.Y;
            Z = state.Z;
            Yaw = state.Yaw;
            Health = state.Health;

            _history.Record(state);
        }

        public PlayerState ExportState()
        {
            return new PlayerState
            {
                PlayerId = PlayerId,
                X = X,
                Y = Y,
                Z = Z,
                Yaw = Yaw,
                Health = Health,
                LastUpdateTick = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
        }

        public PlayerState? GetStateAtTick(long tick) => _history.GetStateAt(tick);
    }
}
