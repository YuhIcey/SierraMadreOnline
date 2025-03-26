using System;
using System.Runtime.InteropServices;

namespace MadreServer.MadreInterop
{
    public static class NativeHooks
    {
        private const string DllName = "MadreClient.dll"; // Must match actual output DLL

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetGodMode(string playerId, bool enabled);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetInfiniteAmmo(string playerId, bool enabled);
    }
}
