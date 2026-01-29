using System.Collections.Generic;
using System.Reflection;
using Comfort.Common;
using EFT;
using SPT.Reflection.Patching;
using MasterKeycard.Scripts;

namespace MasterKeycard.Patches
{
    internal class MasterKeyPatch : ModulePatch
    {
        // 记录已处理的地图（每个游戏会话只初始化一次）
        private static readonly HashSet<string> ProcessedLocations = new HashSet<string>();

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
            
            // 检查是否已处理过此地图
            if (ProcessedLocations.Contains(location))
            {
                MasterKeycardPlugin.LogInfo($"地图 {location} 已配置过主钥匙，跳过");
                return;
            }
            
            // 首次进入该地图，添加到已处理集合
            ProcessedLocations.Add(location);
            MasterKeycardPlugin.LogInfo($"首次进入地图 {location}，初始化主钥匙...");
            
            // 创建并执行初始化
            var masterKeyScript = new MasterKeyScript();
            masterKeyScript.Initialize();
            
            MasterKeycardPlugin.LogInfo($"地图 {location} 主钥匙配置完成");
        }
    }
}
