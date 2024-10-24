using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using UnityEngine;

namespace DevTools
{
    [BepInPlugin("DevTools", "DevTools", "1.0.0")]
    public class DevTools : BasePlugin
    {
        public static ManualLogSource Logger;
        private Harmony harmony;

        public static ConfigEntry<float> walkSpeedMultiplier;
        public static ConfigEntry<float> runSpeedMultiplier;

        public override void Load()
        {
            Logger = Log;
            NewGameHandler eventHandler = new NewGameHandler();
            Logger.LogInfo("Loading DevTools...");
            CommandManager.Initialize();
            try
            {
                harmony = new Harmony("DevTools");
                harmony.PatchAll();
                Logger.LogInfo("All patches applied.");
            }

            catch (Exception ex)
            {
                Logger.LogError($"Error during Load: {ex}");
            }
        }
    }
}
