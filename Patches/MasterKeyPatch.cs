using System.Reflection;
using Comfort.Common;
using EFT;
using SPT.Reflection.Patching;
using MasterKeycard.Scripts;

namespace MasterKeycard.Patches
{
    internal class MasterKeyPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod("OnGameStarted", 
                BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void Postfix()
        {
            // 确保GameWorld已实例化
            if (!Singleton<GameWorld>.Instantiated)
            {
                MasterKeycardPlugin.LogWarning("GameWorld未实例化，无法获取地图信息");
                return;
            }

            // 获取当前地图名称
            string location = Singleton<GameWorld>.Instance.MainPlayer.Location;
            
            // 每次进入地图都初始化主钥匙
            MasterKeycardPlugin.LogInfo($"进入地图 {location}，初始化主钥匙...");
            
            // 创建并执行初始化
            var masterKeyScript = new MasterKeyScript();
            masterKeyScript.Initialize();
            
            MasterKeycardPlugin.LogInfo($"地图 {location} 主钥匙配置完成");
        }
    }
}