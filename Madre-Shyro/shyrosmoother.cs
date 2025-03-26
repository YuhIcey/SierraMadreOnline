using System;

namespace MadreClient.Interpolation
{
    public static class TickSmoother
    {
        private static readonly float tickIntervalMs = 100f; // Match server tick
        private static DateTime lastTickTime = DateTime.UtcNow;

        public static float GetAlpha()
        {
            var now = DateTime.UtcNow;
            float elapsed = (float)(now - lastTickTime).TotalMilliseconds;
            return Math.Clamp(elapsed / tickIntervalMs, 0f, 1f);
        }

        public static void NotifyTickReceived()
        {
            lastTickTime = DateTime.UtcNow;
        }
    }
}
