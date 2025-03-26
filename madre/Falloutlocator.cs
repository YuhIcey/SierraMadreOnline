using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;

namespace MadreServer.Madre
{
    [SupportedOSPlatform("windows")]
    public static class GameLocator
    {
        public static string? FindFalloutNewVegasPath(bool requireUltimateEdition = true)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
                var steamPath = key?.GetValue("SteamPath")?.ToString();

                if (string.IsNullOrEmpty(steamPath))
                {
                    Console.WriteLine("❌ Steam path not found in registry.");
                    return null;
                }

                var gamePath = Path.Combine(steamPath, "steamapps", "common", "Fallout New Vegas");
                if (!Directory.Exists(gamePath))
                {
                    Console.WriteLine("❌ Fallout New Vegas directory not found at expected location.");
                    return null;
                }

                // Check for the existence of the FalloutNV.exe file to verify a legitimate installation.
                string exePath = Path.Combine(gamePath, "FalloutNV.exe");
                if (!File.Exists(exePath))
                {
                    Console.WriteLine("❌ FalloutNV.exe not found in the game directory. This may indicate a pirated or incomplete installation.");
                    return null;
                }

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
                    if (!Directory.Exists(dataPath))
                    {
                        Console.WriteLine("❌ Fallout New Vegas Data folder not found.");
                        return null;
                    }

                    bool allDLCsPresent = requiredDLCs.All(dlc =>
                        File.Exists(Path.Combine(dataPath, dlc)));

                    if (!allDLCsPresent)
                    {
                        Console.WriteLine("⚠️ Ultimate Edition DLCs not detected. Please ensure all required DLCs are installed.");
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
