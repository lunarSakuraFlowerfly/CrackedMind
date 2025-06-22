using UnityEngine;
using NPCSystem.Dialogue;
namespace NPCSystem.Core
{
    /// <summary>
    /// NPC实体接口，定义NPC的基本功能
    /// </summary>
    public interface INPCEntity
    {
           
        /// <summary>
        /// NPC数据
        /// </summary>
        NPCData Data { get; }
    
  
        /// 执行特定行为
        /// </summary>
        /// <param name="behaviorID">行为ID</param>
        void ExecuteBehavior(string behaviorID);
        

        
        /// <summary>
        /// 与该NPC交互
        /// </summary>
        /// <param name="initiator">交互发起者</param>
        void Interact(GameObject initiator);
        
        /// <summary>
        /// 获取NPC位置
        /// </summary>
        Vector3 GetPosition();
        
        /// <summary>
        /// 获取NPC变换组件
        /// </summary>
        Transform GetTransform();

        /// <summary>
        /// 根据对话ID触发特定对话
        /// </summary>
        void TriggerDialogueByID(string dialogueID);

        /// <summary>
        /// 直接触发指定对话数据
        /// </summary>
        void TriggerDialogue(DialogueData dialogueData);

        /// <summary>
        /// 移动到指定位置
        /// </summary>
        void MoveTo(Vector3 position);
    }
} 