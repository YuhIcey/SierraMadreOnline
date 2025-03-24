using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;

namespace MadreServer.Madre
{
    public static class GameLocator
    {
        public static string? FindFalloutNewVegasPath(bool requireUltimateEdition = true)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
                var steamPath = key?.GetValue("SteamPath")?.ToString();

                if (string.IsNullOrEmpty(steamPath))
                    return null;

                var gamePath = Path.Combine(steamPath, "steamapps", "common", "Fallout New Vegas");
                if (!Directory.Exists(gamePath))
                    return null;

                // If user requires Ultimate Edition, check for DLC files
                if (requireUltimateEdition)
                {
                    string[] requiredDLCs =
                    {
                        "DeadMoney.esm",
                        "HonestHearts.esm",
                        "OldWorldBlues.esm",
                        "LonesomeRoad.esm"
                    };

                    string dataPath = Path.Combine(gamePath, "Data");
                    if (!Directory.Exists(dataPath)) return null;

                    bool allDLCsPresent = requiredDLCs.All(dlc =>
                        File.Exists(Path.Combine(dataPath, dlc)));

                    if (!allDLCsPresent)
                    {
                        Console.WriteLine("⚠️ Ultimate Edition DLCs not detected.");
                        return null;
                    }
                }

                return gamePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error locating Fallout NV: {ex.Message}");
                return null;
            }
        }
    }
}
