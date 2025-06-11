using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using InteractionSystem.Item;
using InteractionSystem.Prompt;
using InteractionSystem.Data;
using NPCSystem.Dialogue;
using NPCSystem.Core;
namespace InteractionSystem.Core
{
    /// <summary>
    /// 玩家交互管理器，管理玩家与物品的交互
    /// </summary>
    public class PlayerInteractionManager : MonoBehaviour
    {
        [Header("交互设置")]
        [SerializeField] private float interactionRange = 2f;
        [SerializeField] private LayerMask interactionLayer;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        
        [Header("UI引用")]
        [SerializeField] private InteractionPromptUI promptUI;
        
        [Header("音效设置")]
        [SerializeField] private bool playPromptSound = true; // 是否播放提示音效
        
        private IInteractable currentInteractable;
        private IInteractable lastInteractable;
        private bool isInteracting = false;
        private bool isInDialogue = false; // 新增：标记是否在对话状态
        private bool isInteractionPaused = false; // 新增：是否暂停交互检测
        private float interactionPauseEndTime = 0f; // 新增：交互暂停结束时间
        
        // Start is called before the first frame update
        void Start()
        {
            // 查找交互提示UI
            if (promptUI == null)
            {
                promptUI = FindObjectOfType<InteractionPromptUI>();
                
                if (promptUI == null)
                {
                    Debug.LogWarning("未找到InteractionPromptUI，交互提示将不可用");
                }
            }

            //注册对话开始/结束的事件
            if(DialogueManager.Instance != null)
            {
                DialogueManager.Instance.OnDialogueStart += HandleDialogueStart;
                DialogueManager.Instance.OnDialogueEnd += HandleDialogueEnd;
            }
            else
            {
                Debug.LogWarning("未找到DialogueManager实例，对话时交互提示将不会自动隐藏");
            }
            
            // 注册物品信息UI事件
            ItemInfoUI itemInfoUI = FindObjectOfType<ItemInfoUI>();
            if (itemInfoUI != null)
            {
                // 监听物品信息显示事件（如果ItemInfoUI中有这样的事件）
                // 这里假设ItemInfoUI有OnItemInfoShown和OnItemInfoHidden事件
            }
        }

        private void OnDestroy()
        {
            // 取消事件注册，防止内存泄漏
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.OnDialogueStart -= HandleDialogueStart;
                DialogueManager.Instance.OnDialogueEnd -= HandleDialogueEnd;
            }
        }

        /// <summary>
        /// 处理对话开始事件
        /// </summary>
        private void HandleDialogueStart(NPCData npcData)
        {
            isInDialogue = true;
            
            // 隐藏交互提示UI
            if (promptUI != null)
            {
                promptUI.Hide();
            }
            
            // 禁用玩家控制
            DisablePlayerControl();
        }

        /// <summary>
        /// 处理对话结束事件
        /// </summary>
        public void HandleDialogueEnd()
        {
            isInDialogue = false;
            isInteracting = false; // 确保重置交互状态
            isInteractionPaused = false; // 确保重置交互暂停状态
            
            // 对话结束后不立即显示提示，让Update方法中的CheckForInteractable重新处理
            // 延迟一帧再检测交互物体，避免在对话结束的同一帧立即显示提示
            StartCoroutine(DelayedCheckForInteractable());
            
            // 确保一定会恢复玩家控制
            EnablePlayerControl();
            
            Debug.Log("PlayerInteractionManager: 对话结束，重新启用交互和控制");
        }

        /// <summary>
        /// 延迟检测可交互物体的协程
        /// </summary>
        private IEnumerator DelayedCheckForInteractable()
        {
            // 等待一帧
            yield return null;
            
            // 确保状态重置
            isInteracting = false;
            isInteractionPaused = false;
            
            Debug.Log("PlayerInteractionManager: 延迟一帧后重新检测可交互物体");
            
            // 重新检测可交互物体
            CheckForInteractable();
            
            if (currentInteractable != null)
            {
                Debug.Log($"PlayerInteractionManager: 检测到可交互物体: {currentInteractable.GetTransform().name}");
            }
            else
            {
                Debug.Log("PlayerInteractionManager: 未检测到可交互物体");
            }
        }

        // Update is called once per frame
        void Update()
        {
            // 检查是否需要恢复交互检测
            if (isInteractionPaused)
            {
                if (Time.time >= interactionPauseEndTime)
                {
                    isInteractionPaused = false;
                    // 恢复交互检测后立即检查可交互物体
                    CheckForInteractable();
                }
                else
                {
                    // 暂停交互检测期间，确保提示UI隐藏
                    if (promptUI != null && promptUI.gameObject.activeSelf)
                    {
                        promptUI.Hide();
                    }
                    // 在交互暂停期间，仍然检测交互键输入，但不显示提示
                    CheckInteractionInput();
                    return;
                }
            }
            
            // 如果正在对话中，不进行交互检测
            if (isInDialogue)
            {
                return;
            }
            
            // 如果正在查看物品信息，检查是否需要关闭
            ItemInfoUI itemInfoUI = FindObjectOfType<ItemInfoUI>();
            if (itemInfoUI != null && itemInfoUI.IsInfoShowing())
            {
                if (!isInteracting)
                {
                    // 玩家刚开始查看物品信息，禁用玩家控制
                    DisablePlayerControl();
                }
                
                isInteracting = true;
                return;
            }
            else
            {
                if (isInteracting)
                {
                    // 玩家刚结束查看物品信息，启用玩家控制
                    EnablePlayerControl();
                    isInteracting = false;
                }
            }
            
            // 检测可交互物体
            CheckForInteractable();
            
            // 处理交互输入
            CheckInteractionInput();
        }
        
        /// <summary>
        /// 检查交互输入
        /// </summary>
        private void CheckInteractionInput()
        {
            if (currentInteractable != null && Input.GetKeyDown(interactKey))
            {
                Debug.Log($"PlayerInteractionManager: 检测到交互键按下，当前交互物体: {currentInteractable.GetTransform().name}");
                
                // 检查是否是特殊的不可交互柜子（有提示但不可交互）
                bool isSpecialNonInteractable = false;
                bool isCabinet = false;
                if (currentInteractable is ItemInteractable itemInteractable)
                {
                    ItemData itemData = itemInteractable.GetItemData();
                    if (itemData is CabinetItemData cabinetData)
                    {
                        isCabinet = true;
                        Debug.Log($"PlayerInteractionManager: 检测到柜子，itemID={cabinetData.itemID}, " +
                                  $"isFirstTime={cabinetData.isFirstTime}, " +
                                  $"hasKey={cabinetData.HasKey()}, isOpened={cabinetData.isOpened}");
                                  
                        // 非首次交互且无钥匙的柜子不应该执行交互逻辑
                        if (!cabinetData.isFirstTime && 
                            !cabinetData.HasKey() && // 直接调用柜子的HasKey方法 
                            !cabinetData.isOpened)
                        {
                            isSpecialNonInteractable = true;
                            Debug.Log("PlayerInteractionManager: 检测到特殊不可交互柜子（非首次交互且无钥匙）");
                            
                            // 显示柜子的提示文本，但不执行实际交互
                            Debug.Log($"PlayerInteractionManager: 显示提示 - {cabinetData.lockedPrompt}");
                            
                            // 强制结束本次交互尝试，但不禁用控制
                            return;
                        }
                    }
                }
                
                // 只有非特殊不可交互物体才能真正交互
                if (!isSpecialNonInteractable)
                {
                    // 在交互开始时禁用玩家控制，但保持交互能力
                    DisablePlayerControl();
                    
                    Debug.Log("PlayerInteractionManager: 开始交互");
                    currentInteractable.Interact();
                    
                    // 对于柜子，如果是首次交互，在交互后检查状态变化
                    if (isCabinet && currentInteractable is ItemInteractable itemInteractable2)
                    {
                        ItemData itemData = itemInteractable2.GetItemData();
                        if (itemData is CabinetItemData cabinetData)
                        {
                            Debug.Log($"PlayerInteractionManager: 交互后柜子状态 - " +
                                     $"isFirstTime={cabinetData.isFirstTime}, " +
                                     $"hasKey={cabinetData.HasKey()}, isOpened={cabinetData.isOpened}");
                        }
                    }
                    
                    isInteracting = true;
                    Debug.Log("PlayerInteractionManager: 交互结束，设置isInteracting=true");
                }
                else
                {
                    Debug.Log("PlayerInteractionManager: 特殊不可交互物体，不执行交互");
                }
            }
        }
        
        /// <summary>
        /// 检测可交互物体
        /// </summary>
        private void CheckForInteractable()
        {
            // 如果正在对话中，不检测
            if (isInDialogue) 
            {
                Debug.Log("PlayerInteractionManager: 正在对话中，跳过交互物体检测");
                return;
            }
            
            // 确保不在交互状态
            if (isInteracting)
            {
                Debug.Log("PlayerInteractionManager: 正在交互中，跳过交互物体检测");
                return;
            }
            
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactionLayer);
            
            IInteractable closestInteractable = null;
            float closestDistance = float.MaxValue;
            
            foreach (var collider in colliders)
            {
                IInteractable interactable = collider.GetComponent<IInteractable>();
                
                if (interactable != null)
                {
                    // 首先获取交互提示，即使不可交互也要显示提示
                    string prompt = interactable.GetInteractPrompt();
                    
                    // 如果提示为空，则跳过该物体
                    if (string.IsNullOrEmpty(prompt))
                        continue;
                    
                    // 对于特殊情况，例如"缺少钥匙"，我们仍显示提示但不允许实际交互
                    bool specialNonInteractable = false;
                    if (interactable is ItemInteractable itemInteractable)
                    {
                        ItemData itemData = itemInteractable.GetItemData();
                        if (itemData is CabinetItemData cabinetData)
                        {
                            // 非首次交互且没有钥匙的情况
                            if (!cabinetData.isFirstTime && 
                                !cabinetData.HasKey() && // 直接调用柜子的HasKey方法
                                !cabinetData.isOpened)
                            {
                                specialNonInteractable = true;
                            }
                        }
                    }
                    
                    float distance = Vector2.Distance(transform.position, collider.transform.position);
                    
                    // 对于常规可交互物体或特殊不可交互物体，都要考虑提示显示
                    if (interactable.CanInteract() || specialNonInteractable)
                    {
                        if (distance < closestDistance)
                        {
                            closestInteractable = interactable;
                            closestDistance = distance;
                        }
                    }
                }
            }
            
            // 保存上一个交互对象
            lastInteractable = currentInteractable;
            
            // 如果找到了新的可交互物体，或者当前可交互物体变了
            if (closestInteractable != currentInteractable)
            {
                // 更新当前可交互物体
                currentInteractable = closestInteractable;
                
                // 更新提示UI
                UpdatePromptUI();
                
                // 如果找到了新的可交互物体，播放提示音效
                if (currentInteractable != null && currentInteractable != lastInteractable && playPromptSound)
                {
                    if (InteractionSystem.Audio.InteractionAudioManager.Instance != null)
                    {
                        InteractionSystem.Audio.InteractionAudioManager.Instance.PlayInteractionStartSound();
                    }
                }
            }
            
            // 确保即使可交互物体没有变化，UI状态也是最新的
            else if (currentInteractable != null)
            {
                UpdatePromptUI();
            }
        }
        
        /// <summary>
        /// 更新提示UI
        /// </summary>
        private void UpdatePromptUI()
        {
            if (promptUI == null) return;
            
            // 在对话中或交互暂停时不显示提示UI
            if (isInDialogue || isInteractionPaused)
            {
                promptUI.Hide();
                return;
            }
            
            if (currentInteractable != null)
            {
                // 显示提示，只传递文本和目标位置
                promptUI.Show(currentInteractable.GetInteractPrompt(), currentInteractable.GetTransform());
            }
            else
            {
                promptUI.Hide();
            }
        }
        
        /// <summary>
        /// 暂停交互检测
        /// </summary>
        /// <param name="duration">暂停持续时间(秒)</param>
        public void PauseInteraction(float duration)
        {
            isInteractionPaused = true;
            interactionPauseEndTime = Time.time + duration;
            
            // 立即隐藏提示UI
            if (promptUI != null)
            {
                promptUI.Hide();
            }
        }
        
        /// <summary>
        /// 恢复交互检测
        /// </summary>
        public void ResumeInteraction()
        {
            Debug.Log("PlayerInteractionManager: 恢复交互检测，重置isInteractionPaused=false");
            isInteractionPaused = false;
            
            // 立即检查可交互物体
            CheckForInteractable();
        }
        
        /// <summary>
        /// 重置所有交互状态标志
        /// </summary>
        public void ResetInteractionFlags()
        {
            isInteracting = false;
            isInDialogue = false;
            isInteractionPaused = false;
            
            Debug.Log("PlayerInteractionManager: 手动调用重置所有交互状态标志");
            
            // 立即检查可交互物体
            CheckForInteractable();
            
            // 强制设置时间缩放为1
            Time.timeScale = 1f;
        }
        
        /// <summary>
        /// 重置当前交互的柜子状态（用于测试）
        /// </summary>
        public void ResetCurrentCabinet()
        {
            if (currentInteractable != null && currentInteractable is ItemInteractable itemInteractable)
            {
                ItemData itemData = itemInteractable.GetItemData();
                if (itemData is CabinetItemData cabinetData)
                {
                    // 调用柜子的重置方法
                    cabinetData.ResetToInitialState();
                    Debug.Log($"PlayerInteractionManager: 重置当前交互的柜子 {cabinetData.itemID} 到初始状态");
                    
                    // 重置交互状态
                    ResetInteractionFlags();
                    
                    // 重新检测交互物体
                    CheckForInteractable();
                    
                    return;
                }
            }
            
            Debug.Log("PlayerInteractionManager: 当前没有交互的柜子或交互物体不是柜子");
        }
        
        /// <summary>
        /// 重置所有场景中的柜子（用于测试）
        /// </summary>
        public void ResetAllCabinets()
        {
            // 找到所有带有CabinetItemDataAdapter组件的对象
            CabinetItemDataAdapter[] cabinetAdapters = FindObjectsOfType<CabinetItemDataAdapter>();
            
            if (cabinetAdapters != null && cabinetAdapters.Length > 0)
            {
                Debug.Log($"PlayerInteractionManager: 找到 {cabinetAdapters.Length} 个柜子，准备重置");
                
                foreach (var adapter in cabinetAdapters)
                {
                    if (adapter != null)
                    {
                        // 调用适配器的重置方法
                        adapter.SendMessage("ResetInteractionSystem", null, SendMessageOptions.DontRequireReceiver);
                    }
                }
                
                // 重置交互状态
                ResetInteractionFlags();
                
                // 重新检测交互物体
                CheckForInteractable();
            }
            else
            {
                Debug.Log("PlayerInteractionManager: 场景中没有找到柜子");
            }
        }
        
        /// <summary>
        /// 禁用玩家控制
        /// </summary>
        private void DisablePlayerControl()
        {
            // 查找PlayerControlManager并禁用控制
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                PlayerControlManager controlManager = playerObj.GetComponent<PlayerControlManager>();
                if (controlManager != null)
                {
                    controlManager.DisableControl();
                }
                else
                {
                    Debug.LogWarning("未找到PlayerControlManager组件，无法禁用玩家控制");
                }
            }
            else
            {
                Debug.LogWarning("未找到Player标签的对象，无法禁用玩家控制");
            }
        }
        
        /// <summary>
        /// 启用玩家控制
        /// </summary>
        private void EnablePlayerControl()
        {
            Debug.Log("PlayerInteractionManager: 正在启用玩家控制");
            
            // 查找PlayerControlManager并启用控制
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                PlayerControlManager controlManager = playerObj.GetComponent<PlayerControlManager>();
                if (controlManager != null)
                {
                    // 检查是否可以启用玩家控制（不在对话中且不在查看物品信息）
                    bool canEnableControl = !isInDialogue;
                    
                    ItemInfoUI itemInfoUI = FindObjectOfType<ItemInfoUI>();
                    if (itemInfoUI != null)
                    {
                        canEnableControl = canEnableControl && !itemInfoUI.IsInfoShowing();
                    }
                    
                    if (canEnableControl)
                    {
                        Debug.Log("PlayerInteractionManager: 满足启用玩家控制的条件，正在调用PlayerControlManager.EnableControl()");
                        controlManager.EnableControl();
                    }
                    else
                    {
                        Debug.LogWarning($"PlayerInteractionManager: 无法启用玩家控制，isInDialogue={isInDialogue}");
                    }
                }
                else
                {
                    Debug.LogError("PlayerInteractionManager: 未找到PlayerControlManager组件，无法启用玩家控制");
                }
            }
            else
            {
                Debug.LogError("PlayerInteractionManager: 未找到Player标签的对象，无法启用玩家控制");
            }
            
            // 强制确保时间缩放正常
            Time.timeScale = 1f;
        }
        
        /// <summary>
        /// 在场景视图中显示交互范围
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}
