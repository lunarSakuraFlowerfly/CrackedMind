using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractionSystem.Core
{
    /// <summary>
    /// 所有可交互对象的基础接口
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// 获取交互提示文本
        /// </summary>
        string GetInteractPrompt();

        /// <summary>
        /// 获取交互描述文本
        /// </summary>
        string GetDescription();
        
        /// <summary>
        /// 检查是否可以交互
        /// </summary>
        bool CanInteract();
        
        /// <summary>
        /// 执行交互
        /// </summary>
        void Interact();
        
        /// <summary>
        /// 获取交互对象的变换组件
        /// </summary>
        Transform GetTransform();
    }
}

