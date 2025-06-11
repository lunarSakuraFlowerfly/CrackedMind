using UnityEngine;

namespace NPCSystem.Dialogue
{
    /// <summary>
    /// 对话条件，定义显示对话选项的条件
    /// </summary>
    [System.Serializable]
    public class DialogueCondition
    {
        public enum ConditionType
        {
            HasItem,        // 拥有物品
            CompletedQuest, // 完成任务
            HasFlag,        // 拥有标记
            NPCRelationship // NPC关系值
        }
        
        public ConditionType conditionType;
        public string conditionID; // 物品ID、任务ID、标记ID或NPC ID
        public int requiredValue; // 需要的值（如物品数量、关系值）
        public bool isInverted; // 是否反转条件（如：没有物品）
    }
}