using UnityEngine;
using InteractionSystem.Core;
using NPCSystem.Dialogue;
using System.Collections;
using NPCSystem.Story;
/// <summary>
/// 床的交互组件
/// </summary>
namespace InteractionSystem.Item
{    
    public class BedInteractable : MonoBehaviour, IInteractable
    {
        #region Inspector Fields
        [Header("交互设置")]
        [SerializeField] private string m_InteractPrompt = "睡觉";
        [SerializeField] private bool m_ShowInteractPrompt = true;
        
        [Header("对话设置")]
        [SerializeField] private DialogueData m_FirstInteractionDialogue;
        [SerializeField] private DialogueData m_CannotSleepDialogue;
        [SerializeField] private bool m_HasInteractedBefore = false;
        
        [Header("睡眠设置")]
        [SerializeField] private float m_SleepTransitionDuration = 1.5f;
        [SerializeField] private SleepWakeEffect m_SleepEffect;
        [SerializeField] private RipplePostProcess m_RippleEffect;
        #endregion
        


        
        #region Private Fields
        private bool m_IsInteracting = false;
        #endregion
        
        #region IInteractable Implementation
        public string GetInteractPrompt()
        {
            return m_ShowInteractPrompt ? m_InteractPrompt : string.Empty;
        }
        
        public Transform GetTransform()
        {
            return transform;
        }
        
        public string GetDescription()
        {
            return "一张可以休息的床";
        }
        
        public void Interact()
        {
            // 防止重复交互
            if (m_IsInteracting) return;
            
            m_IsInteracting = true;
            
            // 立即启动波纹扭曲效果
            TriggerRippleEffect();
            
            // 首次交互显示特定对话
            if (!m_HasInteractedBefore && m_FirstInteractionDialogue != null)
            {
                StartFirstInteraction();
                return;
            }
            
            // 检查是否可以睡觉
            if (CanSleep())
            {
                StartSleepSequence();
            }
            else
            {
                ShowCannotSleepDialogue();
            }
        }
        
        public bool CanInteract()
        {
            return !m_IsInteracting;
        }
        #endregion
        
        #region Private Methods
        /// <summary>
        /// 首次交互处理
        /// </summary>
        private void StartFirstInteraction()
        {
            if (DialogueManager.Instance != null && m_FirstInteractionDialogue != null)
            {
                
                // 启动对话
                DialogueManager.Instance.StartDialogue_Item(m_FirstInteractionDialogue);
                
                // 注册对话结束事件
                DialogueManager.Instance.OnDialogueEnd += OnFirstInteractionDialogueEnd;
                
                // 标记为已交互
                m_HasInteractedBefore = true;
            }
            else
            {
                Debug.LogError("无法启动床的首次交互对话，DialogueManager实例不存在或未设置对话数据");
                m_IsInteracting = false;
            }
        }
        
        /// <summary>
        /// 首次交互对话结束回调
        /// </summary>
        private void OnFirstInteractionDialogueEnd()
        {
            // 取消事件注册
            if (DialogueManager.Instance != null)
            {
                StoryManager.Instance.BedFirstTouchComplete();
                Debug.Log("<color=green>床第一次交互完成</color>");
                DialogueManager.Instance.OnDialogueEnd -= OnFirstInteractionDialogueEnd;
            }
            
            // 对话结束后检查是否可以睡觉
            if (CanSleep())
            {
                StartSleepSequence();
            }
            else
            {
                ShowCannotSleepDialogue();
            }
        }
        
        /// <summary>
        /// 显示无法睡觉的对话
        /// </summary>
    private void ShowCannotSleepDialogue()
    {
        string reason = "现在不是睡觉的时候。";
        
        // 从SleepConditionChecker获取具体原因
        if (SleepConditionChecker.Instance != null)
        {
            reason = SleepConditionChecker.Instance.GetCannotSleepReason();
        }
        
        if (DialogueManager.Instance != null)
        {
            // 如果都没有设置，才创建临时对话数据
            if (m_CannotSleepDialogue == null)
            {

                
                // 启动对话
                DialogueManager.Instance.StartDialogue_Item(m_CannotSleepDialogue);
            }
            else
            {
                // 启动对话
                DialogueManager.Instance.StartDialogue_Item(m_CannotSleepDialogue);
            }
            
            // 注册对话结束事件
            DialogueManager.Instance.OnDialogueEnd += OnCannotSleepDialogueEnd;
        }
        else
        {
            Debug.LogError("<color=red>无法显示无法睡觉对话，DialogueManager实例不存在</color>");
            m_IsInteracting = false;
        }
    }
        
        /// <summary>
        /// 无法睡觉对话结束回调
        /// </summary>
        private void OnCannotSleepDialogueEnd()
        {
            // 取消事件注册
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.OnDialogueEnd -= OnCannotSleepDialogueEnd;
            }
            
            // 重置交互状态
            m_IsInteracting = false;
        }
        
        /// <summary>
        /// 开始睡眠序列
        /// </summary>
        public void StartSleepSequence()
        {
            Debug.Log("<color=yellow>开始睡眠序列</color>");
            StartCoroutine(SleepTransition());
        }
        
        /// <summary>
        /// 处理睡眠后的效果
        /// </summary>
        private void HandleAfterSleepEffects()
        {
            // 这里可以添加睡眠后的效果，如恢复体力、刷新任务等
            // 示例：
            // GameManager.Instance.RefreshDailyTasks();
            // PlayerStats.Instance.RestoreEnergy();
        }
        
        /// <summary>
        /// 检查是否可以睡觉
        /// </summary>
        private bool CanSleep()
        {
            //使用sleepConditionChecker检查是否可以睡觉
            if(SleepConditionChecker.Instance != null)
            {
                return SleepConditionChecker.Instance.CanSleep();
            }
            //如果没有找到，就使用默认逻辑
            Debug.LogWarning("无法找到SleepConditionChecker实例，使用默认逻辑");
            return true;
        }
        
        /// <summary>
        /// 禁用玩家控制
        /// </summary>
        private void DisablePlayerControl()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                PlayerControlManager controlManager = playerObj.GetComponent<PlayerControlManager>();
                if (controlManager != null)
                {
                    controlManager.DisableControl();
                }
            }
        }
        
        /// <summary>
        /// 启用玩家控制
        /// </summary>
        private void EnablePlayerControl()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                PlayerControlManager controlManager = playerObj.GetComponent<PlayerControlManager>();
                if (controlManager != null)
                {
                    controlManager.EnableControl();
                }
            }
        }
        
        /// <summary>
        /// 触发波纹扭曲效果
        /// </summary>
        private void TriggerRippleEffect()
        {
            // 查找并触发波纹效果
            if (m_RippleEffect == null)
            {
                m_RippleEffect = FindObjectOfType<RipplePostProcess>();
            }
            
            if (m_RippleEffect != null)
            {
                m_RippleEffect.CaptureSceneSnapshot(); // 先捕获当前场景
                m_RippleEffect.EnableEffect(0.3f, 0.8f); // 快速启用扭曲效果，使用较高强度
                Debug.Log("<color=cyan>已触发波纹扭曲效果</color>");
            }
            else
            {
                Debug.LogWarning("<color=yellow>找不到RipplePostProcess组件，无法触发扭曲效果</color>");
            }
        }
        
        private IEnumerator SleepTransition()
        {
            // 禁用玩家控制
            DisablePlayerControl();
            
            Debug.Log("<color=yellow>====== 开始睡眠过渡 ======</color>");
            
            // 直接使用SleepWakeEffect的波纹和淡入淡出效果
            if (m_SleepEffect != null)
            {
                Debug.Log("<color=green>调用SleepEffect.StartSleep()</color>");
                m_SleepEffect.StartSleep();
                
                // 使用协程等待2秒，确保流程不会提前完成
                yield return new WaitForSeconds(2.0f);
            }
            else
            {
                Debug.LogError("<color=red>SleepWakeEffect组件未设置！</color>");
                
                // 自动查找场景中的SleepWakeEffect组件
                m_SleepEffect = FindObjectOfType<SleepWakeEffect>();
                if (m_SleepEffect != null)
                {
                    Debug.Log("<color=green>自动找到SleepWakeEffect组件</color>");
                    m_SleepEffect.StartSleep();
                    
                    // 使用协程等待2秒，确保流程不会提前完成
                    yield return new WaitForSeconds(2.0f);
                }
                else
                {
                    // 增加天数
                    Debug.Log("<color=yellow>无法找到SleepWakeEffect组件，使用备用睡眠流程</color>");
    
                    
                    // 等待过渡时间
                    yield return new WaitForSeconds(m_SleepTransitionDuration);
                    
                    // 恢复玩家控制
                    EnablePlayerControl();
                }
            }
            
            Debug.Log("<color=yellow>====== 睡眠过渡结束 ======</color>");
            
            // 确保波纹效果结束
            if (m_RippleEffect != null)
            {
                m_RippleEffect.DisableEffect();
            }
            
            // 重置交互状态
            m_IsInteracting = false;
        }
        #endregion
    }
}