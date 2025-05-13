using UnityEngine;

namespace NPCSystem.Core
{
    /// <summary>
    /// NPC可交互接口，所有可交互的NPC必须实现此接口
    /// </summary>
    public interface INPCInteractable
    {
        /// <summary>
        /// 获取交互提示文本
        /// </summary>
        string GetInteractPrompt();
        
        /// <summary>
        /// 检查是否可以交互
        /// </summary>
        bool CanInteract();
        
        /// <summary>
        /// 执行交互
        /// </summary>
        void Interact();
        
        /// <summary>
        /// 获取NPC的变换组件
        /// </summary>
        Transform GetTransform();
        
        /// <summary>
        /// 获取NPC数据
        /// </summary>
        NPCData GetNPCData();
    }
}