using UnityEngine;
using System.Collections.Generic;
using NPCSystem.Core;

namespace IneractionSystem.Core
{
    /// <summary>
    /// NPC辅助类，提供创建空行为的方法
    /// </summary>
    public static class NPCHelper
    {
        /// <summary>
        /// 创建一个空的NPC行为
        /// </summary>
        /// <param name="behaviorIndex">行为索引，用于生成ID</param>
        /// <returns>新创建的空行为</returns>
        public static NPCBehaviorData CreateEmptyBehavior(int behaviorIndex)
        {
            return new NPCBehaviorData
            {
                behaviorID = "behavior_" + (behaviorIndex + 1).ToString("D3"),
                behaviorName = "",
                behaviorType = NPCBehaviorType.Move,
                targetTag = "",
                targetName = "",
                duration = 0f,
                animationTrigger = "",
                soundEffect = null
            };
        }
        
        /// <summary>
        /// 为NPC数据添加一个空行为
        /// </summary>
        /// <param name="npcData">NPC数据</param>
        /// <returns>新添加的行为索引</returns>
        public static int AddEmptyBehaviorToNPC(NPCData npcData)
        {
            if (npcData == null)
                return -1;
                
            NPCBehaviorData newBehavior = CreateEmptyBehavior(npcData.behaviors.Count);
            npcData.behaviors.Add(newBehavior);
            
            return npcData.behaviors.Count - 1;
        }
    }
}