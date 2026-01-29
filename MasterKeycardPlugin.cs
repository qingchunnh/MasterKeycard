using BepInEx;
using BepInEx.Logging;
using MasterKeycard.Patches;
using UnityEngine;

namespace MasterKeycard
{
    [BepInPlugin("com.qingchun.masterkeycard", "MasterKeycard", "1.0.1")]
    public class MasterKeycardPlugin : BaseUnityPlugin
    {
        internal new static ManualLogSource Logger { get; private set; }

        public void Awake()
        {
            Logger = base.Logger;
            new MasterKeyPatch().Enable();  // 启用 Patch
        }

        public static void LogInfo(string message)
        {
            Logger.LogInfo(message);
        }

        public static void LogError(string message)
        {
            Logger.LogError(message);
        }

        public static void LogWarning(string message)
        {
            Logger.LogWarning(message);
        }
    }
}