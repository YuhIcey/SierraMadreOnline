using System;
using System.Collections.Generic;

namespace MadreServer.Hasher
{
    public static class AdminToken
    {
        private static readonly Dictionary<string, string> ActiveTokens = new();

        public static string GenerateToken(string adminName)
        {
            var raw = $"{adminName}:{DateTime.UtcNow.Ticks}";
            var token = Hasher.Sha256Hash(raw);
            ActiveTokens[adminName] = token;
            return token;
        }

        public static bool ValidateToken(string adminName, string token)
        {
            return ActiveTokens.TryGetValue(adminName, out var realToken) && realToken == token;
        }

        public static void InvalidateToken(string adminName)
        {
            if (ActiveTokens.ContainsKey(adminName))
                ActiveTokens.Remove(adminName);
        }
    }
}
