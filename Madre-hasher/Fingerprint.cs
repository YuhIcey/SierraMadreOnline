using System;
using System.IO;
using MadreServer.Hasher; // Ensure this matches the actual Hasher namespace

namespace MadreServer.Hasher
{
    public static class Fingerprint
    {
        public static string GetPlayerFingerprint(string playerId, string machineInfo)
        {
            return Hasher.Sha256Hash(playerId + "::" + machineInfo);
        }

        public static string GetPluginFingerprint(string pluginPath)
        {
            if (!File.Exists(pluginPath)) return string.Empty;

            var bytes = File.ReadAllBytes(pluginPath);
            return Hasher.Sha256Hash(Convert.ToBase64String(bytes));
        }
    }
}
