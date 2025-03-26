namespace MadreServer.MadreShyro
{
    public class PlayerState
    {
        // Unique player identity (e.g. SteamID, session token, etc.)
        public string PlayerId { get; set; } = string.Empty;

        // World position
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        // Orientation
        public float Pitch { get; set; }
        public float Yaw { get; set; }
        public float Roll { get; set; }

        // Movement state
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public float VelocityZ { get; set; }

        // Animation state
        public int AnimState { get; set; }  // 0 = idle, 1 = walk, 2 = run, 3 = jump, etc.

        // Combat
        public int WeaponId { get; set; }
        public int Health { get; set; } = 100;
        public int AP { get; set; } = 100;

        // Status flags
        public bool IsCrouching { get; set; }
        public bool IsShooting { get; set; }
        public bool IsReloading { get; set; }

        // Tick index for interpolation/prediction sync
        public long LastUpdateTick { get; set; }
    }
}
