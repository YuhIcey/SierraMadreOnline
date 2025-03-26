using System.Runtime.InteropServices;

namespace MadreServer.MadreInterop
{
    public static class INetNative
    {
        [DllImport("MadreClient.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetGodMode(string playerId, bool enabled);

        [DllImport("MadreClient.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetInfiniteAmmo(string playerId, bool enabled);

        [DllImport("MadreClient.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void TeleportTo(string playerId, float x, float y, float z);
    }
}
