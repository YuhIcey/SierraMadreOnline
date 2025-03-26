namespace MadreServer.MadreShared
{
    public class PlayerState
    {
        public string PlayerId { get; set; } = string.Empty;

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }

        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public float VelocityZ { get; set; }

        public int AnimState { get; set; }

        public int WeaponId { get; set; }
        public int Health { get; set; } = 100;
        public int AP { get; set; } = 100;

        public bool IsCrouching { get; set; }
        public bool IsShooting { get; set; }
        public bool IsReloading { get; set; }

        public bool PvPEnabled { get; set; }
        public bool IsInSafeZone { get; set; }

        public string Faction { get; set; } = string.Empty;

        public long LastUpdateTick { get; set; }
    }
}
