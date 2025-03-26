using System;
using System.Collections.Generic;
using System.Numerics;
using MadreServer.MadreShared;
using MadreClient.Interpolation;

namespace MadreClient.Render
{
    public class NetPlayerRenderer
    {
        private readonly string _playerId;
        private readonly Queue<PlayerState> _stateBuffer = new();
        private const int MaxBufferSize = 5;

        public Vector3 CurrentRenderPosition { get; private set; }
        public float CurrentYaw { get; private set; }

        public NetPlayerRenderer(string playerId)
        {
            _playerId = playerId;
        }

        public void EnqueueState(PlayerState state)
        {
            _stateBuffer.Enqueue(state);
            if (_stateBuffer.Count > MaxBufferSize)
                _stateBuffer.Dequeue();

            TickSmoother.NotifyTickReceived(); // Mark new tick received
        }

        public void Update()
        {
            if (_stateBuffer.Count < 2) return;

            var states = _stateBuffer.ToArray();
            var from = states[0];
            var to = states[1];
            float t = TickSmoother.GetAlpha();

            CurrentRenderPosition = Vector3.Lerp(
                new Vector3(from.X, from.Y, from.Z),
                new Vector3(to.X, to.Y, to.Z),
                t
            );

            CurrentYaw = Lerp(from.Yaw, to.Yaw, t);
        }

        public void Render()
        {
            // This is where you draw the ghost NPC or player proxy
            // Example: Engine.RenderGhost(_playerId, CurrentRenderPosition, CurrentYaw);
        }

        private static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }
}
