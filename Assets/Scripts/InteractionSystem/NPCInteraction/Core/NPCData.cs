using UnityEngine;
using NPCSystem.Dialogue;
using System.Collections.Generic;
using NPCSystem.Behavior;

namespace NPCSystem.Core
{
    /// <summary>
    /// NPC数据，定义NPC的基本信息和对话数据
    /// </summary>
    [CreateAssetMenu(fileName = "New NPC Data", menuName = "NPCs/NPC Data")]
    public class NPCData : ScriptableObject
    {
        [SerializeField] private string m_npcID;  // 添加私有字段
        public string npcID 
        { 
            get => m_npcID;
            set => m_npcID = value;
        }

        [SerializeField] private string m_npcName;  // 添加私有字段
        public string npcName
        {
            get => m_npcName;
            set => m_npcName = value;
        }

        [TextArea]
        public string description;
        public Sprite portrait;
        
        [Header("交互设置")]
        public string interactPrompt = "按E交谈";
        
        //[Header("对话数据")]
        //public DialogueData dialogueData;

        [Tooltip("多对话数据集合，使用对话ID时使用")]
        [SerializeField]
        public List<DialogueData> dialogueDatas = new List<DialogueData>();
        
        [Header("行为设置")]
        public List<NPCBehaviorData> behaviors = new List<NPCBehaviorData>();
        
        // 用于跟踪行为数量变化
        [HideInInspector]
        public int lastBehaviorCount = 0;
        
        private void OnValidate()
        {
            // 确保NPC ID不为空
            if (string.IsNullOrEmpty(m_npcID))
            {
                m_npcID = name.Replace(" ", "_").ToLower();
            }

            // 确保NPC名称不为空
            if (string.IsNullOrEmpty(m_npcName))
            {
                m_npcName = name;
            }

            // 检查行为列表变化
            if (behaviors.Count > lastBehaviorCount)
            {
                // 初始化新添加的行为
                for (int i = lastBehaviorCount; i < behaviors.Count; i++)
                {
                    if (string.IsNullOrEmpty(behaviors[i].behaviorID))
                    {
                        behaviors[i].behaviorID = $"behavior_{(i + 1):D3}";
                    }
                }
            }
            lastBehaviorCount = behaviors.Count;
        }
    }
    
    /// <summary>
    /// NPC行为数据
    /// </summary>
    [System.Serializable]
    public class NPCBehaviorData
    {
        public string behaviorID;
        public string behaviorName;
        public NPCBehaviorType behaviorType;
        
        [Header("目标位置设置")]
        [Tooltip("目标对象的Tag（优先使用）")]
        public string targetTag;
        [Tooltip("目标对象的名称（当Tag为空时使用）")]
        public string targetName;
        [Tooltip("如果两者都为空，将使用当前位置")]
        
        public float duration = 0f;
        public string animationTrigger;
        public AudioClip soundEffect;
        
        /// <summary>
        /// 在运行时获取目标Transform
        /// </summary>
        public Transform GetTargetTransform()
        {
            // 优先使用Tag查找
            if (!string.IsNullOrEmpty(targetTag))
            {
                GameObject targetObj = GameObject.FindWithTag(targetTag);
                if (targetObj != null)
                {
                    return targetObj.transform;
                }
            }
            
            // 如果没有找到或没有Tag，使用名称查找
            if (!string.IsNullOrEmpty(targetName))
            {
                GameObject targetObj = GameObject.Find(targetName);
                if (targetObj != null)
                {
                    return targetObj.transform;
                }
            }
            
            return null;
        }
    }
    
    /// <summary>
    /// NPC行为类型
    /// </summary>
    public enum NPCBehaviorType
    {
        Move,       // 移动到指定位置
        Sit,        // 坐下
        Wait,       // 等待
        OpenDoor,   // 开门
        Cook,       // 做饭
        Animation,  // 播放动画
        Custom      // 自定义行为
    }
}