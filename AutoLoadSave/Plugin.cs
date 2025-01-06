using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace AutoLoadSave
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class AutoLoadSave : BaseUnityPlugin
    {
        public enum SaveType
        {
            OpenShift, Standard, Limited, NoRevival
        }

        private const string modGUID = "86maylin.auto_load_save";
        private const string modName = "Automatically load career save when starting the game";
        private const string modVersion = "1.0.0.0";

        public static AutoLoadSave Instance { get; private set; }
        private readonly Harmony harmony = new Harmony(modGUID);

        public static ManualLogSource LoggerInstance;
        public static ConfigEntry<bool> Config_enabled { get; private set; }
        public static ConfigEntry<int> Config_shiftType { get; private set; }
        public static ConfigEntry<bool> Config_drainDisabled { get; private set; }
        public static bool HasLoadedSave = false;

        void Awake()
        {
            Instance = this;
            LoggerInstance = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            SetConfigs();
            if (Config_enabled.Value)
            {
                harmony.PatchAll();
                LoggerInstance.LogInfo("AutoLoadSave patched.");
            }
            else
            {
                LoggerInstance.LogInfo("AutoLoadSave disabled.");
            }
        }

        private void SetConfigs()
        {
            Config_enabled = Config.Bind("Config", "Enabled", true, "Whether to enable the plugin or not.");
            Config_shiftType = Config.Bind("Config", "ShiftType", 1, "Shift type. 0: OpenShift 1: Standard 2: Limited 3: No Revival");
            Config_drainDisabled = Config.Bind("Config", "DrainDisabled", false, "Whether to disable oxygen drain for OpenShift mode or not.");
        }
    }
}
