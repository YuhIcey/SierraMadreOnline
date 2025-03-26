using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace MadreServer.Plugins
{
    public interface IShyroPlugin
    {
        string Name { get; }
        string Version { get; }
        void OnLoad();
        void OnTick();
        void OnShutdown();
    }

    public static class PluginLoader
    {
        private static readonly List<IShyroPlugin> LoadedPlugins = new();

        public static void LoadAll(string pluginDir, List<string> enabledList)
        {
            Console.WriteLine($"🔌 Loading plugins from: {pluginDir}");

            foreach (string pluginName in enabledList)
            {
                string dllPath = Path.Combine(pluginDir, pluginName + ".dll");
                string metaPath = Path.Combine(pluginDir, pluginName, "plugin.json");

                if (!File.Exists(dllPath))
                {
                    Console.WriteLine($"❌ Plugin missing: {dllPath}");
                    continue;
                }

                try
                {
                    var asm = Assembly.LoadFile(Path.GetFullPath(dllPath));
                    foreach (Type type in asm.GetTypes())
                    {
                        if (typeof(IShyroPlugin).IsAssignableFrom(type) && !type.IsInterface)
                        {
                            var plugin = (IShyroPlugin)Activator.CreateInstance(type)!;
                            plugin.OnLoad();
                            LoadedPlugins.Add(plugin);

                            Console.WriteLine($"✅ Loaded: {plugin.Name} v{plugin.Version}");

                            if (File.Exists(metaPath))
                            {
                                var meta = JObject.Parse(File.ReadAllText(metaPath));
                                string nexus = meta["nexusUrl"]?.ToString() ?? "";
                                if (!string.IsNullOrEmpty(nexus))
                                    Console.WriteLine($"🌐 Nexus: {nexus}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to load plugin {pluginName}: {ex.Message}");
                }
            }
        }

        public static void TickAll()
        {
            foreach (var plugin in LoadedPlugins)
            {
                plugin.OnTick();
            }
        }

        public static void ShutdownAll()
        {
            foreach (var plugin in LoadedPlugins)
            {
                plugin.OnShutdown();
            }
        }
    }
}
