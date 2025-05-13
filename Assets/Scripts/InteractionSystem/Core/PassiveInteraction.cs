using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using InteractionSystem.Prompt;
using InteractionSystem.Core;

namespace InteractionSystem.Core
{
    /// <summary>
    /// 被动交互组件，当玩家进入指定范围时自动触发交互
    /// </summary>
    public class PassiveInteraction : MonoBehaviour
    {
        [Header("交互设置")]
        [SerializeField] private float passiveInteractionRange = 3f; // 被动交互范围，通常大于主动交互范围
        [SerializeField] private LayerMask playerLayer; // 玩家层级
        [SerializeField] private bool triggerOnce = true; // 是否只触发一次
        [SerializeField] private float interactionCooldown = 5f; // 交互冷却时间(秒)
        
        [Header("控制设置")]
        [SerializeField] private bool disablePlayerControl = true; // 是否在交互时禁用玩家控制
        [SerializeField] private float controlDisableDuration = 5f; // 禁用玩家控制的持续时间
        
        [Header("事件")]
        public UnityEvent OnPassiveInteractionTriggered; // 被动交互触发时的事件
        
        // 状态变量
        private bool hasInteracted = false; // 是否已经交互过
        private float lastInteractionTime = -999f; // 上次交互时间
        private Transform playerTransform; // 玩家变换组件引用
        private InteractionPromptUI promptUI; // 提示UI引用
        private bool isControlDisabled = false; // 是否已禁用玩家控制
        private GameObject playerObject; // 玩家游戏对象引用
        
        private void Start()
        {
            // 查找玩家
            playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
            else
            {
                Debug.LogWarning("未找到玩家对象，请确保玩家对象已标记为'Player'标签");
            }
            
            // 查找交互提示UI
            promptUI = FindObjectOfType<InteractionPromptUI>();
            if (promptUI == null)
            {
                Debug.LogWarning("未找到InteractionPromptUI，提示UI将无法隐藏");
            }
        }
        
        private void Update()
        {
            // 如果已经交互过且只触发一次，则不再检测
            if (hasInteracted && triggerOnce) return;
            
            // 检查冷却时间
            if (Time.time - lastInteractionTime < interactionCooldown) return;
            
            // 如果找到了玩家，检查距离
            if (playerTransform != null)
            {
                float distance = Vector2.Distance(transform.position, playerTransform.position);
                
                // 如果玩家在被动交互范围内
                if (distance <= passiveInteractionRange)
                {
                    // 触发被动交互
                    TriggerPassiveInteraction();
                }
            }
            
            // 检查是否需要恢复玩家控制
            if (isControlDisabled && Time.time - lastInteractionTime >= controlDisableDuration)
            {
                EnablePlayerControl();
            }
        }
        
        /// <summary>
        /// 触发被动交互
        /// </summary>
        private void TriggerPassiveInteraction()
        {
            // 更新交互状态
            hasInteracted = true;
            lastInteractionTime = Time.time;
            
            // 隐藏交互提示UI
            HidePromptUI();
            
            // 禁用玩家控制
            if (disablePlayerControl)
            {
                DisablePlayerControl();
            }
            
            // 触发事件
            OnPassiveInteractionTriggered?.Invoke();
        }
        
        /// <summary>
        /// 隐藏交互提示UI
        /// </summary>
        private void HidePromptUI()
        {
            if (promptUI != null)
            {
                promptUI.Hide();
            }
            
            // 找到玩家交互管理器
            PlayerInteractionManager interactionManager = FindObjectOfType<PlayerInteractionManager>();
            if (interactionManager != null)
            {
                // 使用发送消息调用暂停交互检测的方法
                interactionManager.PauseInteraction(controlDisableDuration);
            }
        }
        
        /// <summary>
        /// 禁用玩家控制
        /// </summary>
        private void DisablePlayerControl()
        {
            if (playerObject != null)
            {
                // 尝试获取PlayerControlManager
                var controlManager = playerObject.GetComponent<PlayerControlManager>();
                if (controlManager != null)
                {
                    controlManager.DisableControl();
                    isControlDisabled = true;
                }
                else
                {
                    Debug.LogWarning("未找到PlayerControlManager组件，无法禁用玩家控制");
                }
            }
        }
        
        /// <summary>
        /// 启用玩家控制
        /// </summary>
        private void EnablePlayerControl()
        {
            if (!isControlDisabled) return;
            
            if (playerObject != null)
            {
                // 尝试获取PlayerControlManager
                var controlManager = playerObject.GetComponent<PlayerControlManager>();
                if (controlManager != null)
                {
                    controlManager.EnableControl();
                    isControlDisabled = false;
                }
            }
        }
        
        /// <summary>
        /// 重置交互状态，允许再次交互
        /// </summary>
        public void ResetInteraction()
        {
            hasInteracted = false;
        }
        
        /// <summary>
        /// 在场景视图中显示被动交互范围
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, passiveInteractionRange);
        }
    }
} 