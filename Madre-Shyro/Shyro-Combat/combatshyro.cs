using MadreServer.MadreNet;
using System;

namespace MadreServer.MadreCombat
{
    public static class CombatHandler
    {
        public static bool HandleShoot(string shooterId, string targetId, long shootTick)
        {
            var shooter = NetPlugin.Instance.GetPlayer(shooterId);
            var target = NetPlugin.Instance.GetPlayer(targetId);

            if (shooter == null || target == null) return false;

            var historicalTarget = target.GetStateAtTick(shootTick);
            if (historicalTarget == null)
            {
                Console.WriteLine("⚠️ No historical state available for target.");
                return false;
            }

            var dx = historicalTarget.X - shooter.X;
            var dy = historicalTarget.Y - shooter.Y;
            var dz = historicalTarget.Z - shooter.Z;

            float distance = MathF.Sqrt(dx * dx + dy * dy + dz * dz);
            if (distance <= 3f)
            {
                Console.WriteLine($"🎯 Hit registered by {shooterId} on {targetId}");
                // Register hit logic here (reduce health etc)
                return true;
            }

            Console.WriteLine($"🛑 Shot missed at tick {shootTick} due to distance: {distance}");
            return false;
        }
    }
}
