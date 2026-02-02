using BepInEx;
using BepInEx.Logging;
using Steamworks;
using Steamworks.Ugc;
using HarmonyLib;
using MonoMod.Utils;
using System.Reflection;
using System;
using System.IO;

namespace AssScaner
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class AssScaner : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
#pragma warning disable RCS1213 // Remove unused member declaration
        private void Awake()
        {
            // startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            Logger.LogInfo($"Starting to patch shit");
            var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase;
            var compilemethod = ReflectionHelper.GetType("BackgroundItemLoader").GetMethod("ProcessModCompilationResult", flags);
            Logger.LogInfo($"shit -> {compilemethod?.Name ?? "No shit?"}");
            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

            harmony.Patch(
                compilemethod,
                postfix: new HarmonyMethod(typeof(AssScaner), nameof(PmcrPostfix))
                );
            Logger.LogInfo("Patched shit");
        }
#pragma warning restore RCS1213 // Remove unused member declaration

        public static void PmcrPostfix((ModCompilationResult result, ModScript script) tuple, ModMetaData mod)
        {
            string location = mod.MetaLocation;
            Logger.LogInfo($"Checking mod {mod.Name} in {location}.");
            foreach (var file in Directory.GetFiles(location, "*.dll", SearchOption.AllDirectories))
            {
                Logger.LogInfo($"Found assembly {file}...");
                var ass = Assembly.LoadFile(file);
                if (ass != null)
                {
                    foreach (var referenced in ass.GetReferencedAssemblies())
                    {
                        Logger.LogInfo($"... That uses {referenced}");
                        if (referenced.Name == "Facepunch.Steamworks.Win64")
                        {
                            Logger.LogInfo(".... And that is possibly evil!!!");
                            ModLoader.ModScripts.Remove(mod);
                            mod.Active = false;
                            mod.HasErrors = true;
                            //mod.Suspicious = true;
                            // раскомментировать нижнее когда я изобрету доверие
                            //mod.GetType().GetProperty("Suspicious", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(mod, true);
                            mod.Errors = "Disabled by AssScaner since it is possibly dangerous!";
                            return;
                        }
                    }
                }
            }
        }
    }
}
