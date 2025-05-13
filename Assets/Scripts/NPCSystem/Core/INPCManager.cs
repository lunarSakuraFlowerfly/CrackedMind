using System.Collections.Generic;
using UnityEngine;

namespace NPCSystem.Core
{
    /// <summary>
    /// NPC管理器接口，定义NPC管理的基本功能
    /// </summary>
    public interface INPCManager
    {
        /// <summary>
        /// 根据ID获取NPC实体
        /// </summary>
        /// <param name="npcID">NPC的唯一标识符</param>
        /// <returns>NPC实体，如果不存在则返回null</returns>
        INPCEntity GetNPC(string npcID);
        
        /// <summary>
        /// 生成NPC到场景
        /// </summary>
        /// <param name="npcID">NPC的唯一标识符</param>
        /// <param name="spawnPoint">生成位置</param>
        /// <returns>生成的NPC实体</returns>
        INPCEntity SpawnNPC(string npcID, Transform spawnPoint);
        
        /// <summary>
        /// 从场景移除NPC
        /// </summary>
        /// <param name="npcID">NPC的唯一标识符</param>
        void DespawnNPC(string npcID);
        
        /// <summary>
        /// 获取场景中的所有NPC
        /// </summary>
        /// <returns>NPC实体列表</returns>
        List<INPCEntity> GetAllNPCs();
        
        /// <summary>
        /// 更新所有NPC
        /// </summary>
        void UpdateAllNPCs();
    }

   
}   