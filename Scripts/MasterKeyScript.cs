using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT.Interactive;
using MasterKeycard.Helpers;
using UnityEngine;

namespace MasterKeycard.Scripts
{
    public class MasterKeyScript
    {
        public void Initialize()
        {
            // 查找场景中所有 KeycardDoor
            List<KeycardDoor> keycardDoors = UnityEngine.Object.FindObjectsOfType<KeycardDoor>().ToList();
            
            MasterKeycardPlugin.LogInfo($"找到 {keycardDoors.Count} 个门卡门");
            
            if (keycardDoors.Count == 0)
            {
                MasterKeycardPlugin.LogWarning("未找到任何门卡门");
                return;
            }
            
            int processedCount = 0;
            string masterKeyId = MasterKeyHelper.GetMasterKey();
            
            foreach (var door in keycardDoors)
            {
                // 安全检查
                if (door == null) continue;
                
                // 只处理有 KeyId 的门
                if (!string.IsNullOrEmpty(door.KeyId))
                {
                    if (AddMasterKeyToDoor(door, masterKeyId))
                    {
                        processedCount++;
                    }
                }
            }
            
            MasterKeycardPlugin.LogInfo($"主钥匙已配置，共处理 {processedCount} 个门卡门");
        }
        
        private bool AddMasterKeyToDoor(KeycardDoor door, string masterKeyId)
        {
            // 反射获取 _additionalKeys 字段
            FieldInfo additionalKeysField = typeof(KeycardDoor).GetField("_additionalKeys", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            if (additionalKeysField == null)
            {
                MasterKeycardPlugin.LogError("无法获取 _additionalKeys 字段");
                return false;
            }
            
            // 获取当前值
            string[] currentAdditionalKeys = additionalKeysField.GetValue(door) as string[];
            List<string> newAdditionalKeys = currentAdditionalKeys?.ToList() ?? new List<string>();
            
            // 避免重复添加
            if (!newAdditionalKeys.Contains(masterKeyId))
            {
                newAdditionalKeys.Add(masterKeyId);
                additionalKeysField.SetValue(door, newAdditionalKeys.ToArray());
                return true;
            }
            
            return false;
        }
    }
}
