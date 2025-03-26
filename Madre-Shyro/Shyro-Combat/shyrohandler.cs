using MadreServer.MadreNet;
using MadreServer.MadreShared;
using System;

namespace MadreServer.ShryoCombat 
{
    public static class ShyroCombatHandler
    {
        public static bool HandleShoot(INetPlugin net, string shooterId, string targetId, long shootTick)
        {
            var shooter = net.GetPlayer(shooterId);
            var target = net.GetPlayer(targetId);

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
                return true;
            }

            Console.WriteLine($"🛑 Shot missed at tick {shootTick} due to distance: {distance}");
            return false;
        }
    }
}
