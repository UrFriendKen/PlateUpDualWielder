using System.Reflection;
using UnityEngine;

namespace KitchenDualWielder
{
    public class Main : BaseMain
    {
        public const string MOD_GUID = $"IcedMilo.PlateUp.{MOD_NAME}";
        public const string MOD_NAME = "Dual Wielder";
        public const string MOD_VERSION = "0.1.2";

        public Main() : base(MOD_GUID, MOD_NAME, MOD_VERSION, Assembly.GetExecutingAssembly())
        {
        }

        public override void OnPostActivate(KitchenMods.Mod mod)
        {
        }

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
