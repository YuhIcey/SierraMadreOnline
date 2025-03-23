using System;
using System.IO;

namespace MadreServer.Hasher
{
    public static class Fingerprint
    {
        public static string GetPlayerFingerprint(string playerId, string machineInfo)
        {
            return Hasher.SHA256(playerId + "::" + machineInfo);
        }

        public static string GetPluginFingerprint(string pluginPath)
        {
            if (!File.Exists(pluginPath)) return string.Empty;

            var bytes = File.ReadAllBytes(pluginPath);
            return Hasher.SHA256(Convert.ToBase64String(bytes));
        }
    }
}
