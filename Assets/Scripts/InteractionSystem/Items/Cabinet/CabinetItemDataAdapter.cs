using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractionSystem.Core;
using InteractionSystem.Data;
using NPCSystem.Dialogue;
using NPCSystem.Core;
using InteractionSystem.Item;
using UnityEditor.ShaderGraph.Drawing.Inspector.PropertyDrawers;
using NPCSystem.Story;

/// <summary>
/// CabinetItemData适配器，用于将柜子数据与被动交互系统集成
/// </summary>
[RequireComponent(typeof(PassiveInteraction))]
public class CabinetItemDataAdapter : MonoBehaviour
{
    [Header("柜子数据")]
    [SerializeField] private CabinetItemData cabinetData;
    
    private PassiveInteraction passiveInteraction;
    private bool hasProcessedPassiveInteraction = false;
    private ItemInteractable itemInteractable;

    
    private void Awake()
    {
        // 获取被动交互组件
        passiveInteraction = GetComponent<PassiveInteraction>();
        
        // 获取物品交互组件
        itemInteractable = GetComponent<ItemInteractable>();
        
        // 注册事件
        if (passiveInteraction != null)
        {
            passiveInteraction.OnPassiveInteractionTriggered.AddListener(HandlePassiveInteraction);
        }
    }
    
    private void Start()
    {
        // 确保CabinetItemData引用有效
        if (cabinetData == null)
        {
            Debug.LogError("CabinetItemDataAdapter: 未设置CabinetItemData！", this);
            return;
        }
        
        
        // 确保柜子数据初始化为首次交互状态（适用于测试）
        if (cabinetData != null)
        {
            // 使用完整的重置方法
            cabinetData.ResetToInitialState();
        }
        
        // 立即对交互系统进行一次状态重置
        Invoke("ResetInteractionSystem", 1f);


    }
    
    private void Update()
    {
        // 如果已经处理过被动交互，检查玩家是否在附近
        if (hasProcessedPassiveInteraction)
        {
            // 获取玩家对象
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // 计算与玩家的距离
                float distance = Vector3.Distance(transform.position, player.transform.position);
                
                // 如果玩家离开了柜子一定距离，重置状态，让玩家能再次交互
                if (distance > 5f) // 5米距离
                {
                    // 设置为未处理被动交互的状态，以便玩家再次接近时可以触发
                    hasProcessedPassiveInteraction = false;
                    
                    // 重置交互系统状态
                    ResetInteractionSystem();
                    
                }
            }
        }
        
        // 定期检查CabinetItemData状态
        if (Time.frameCount % 60 == 0 && cabinetData != null) // 每60帧检查一次
        {
            SyncCabinetItemState();
        }
        
        // 测试功能：按R键重置柜子状态
        if (Input.GetKeyDown(KeyCode.R) && cabinetData != null)
        {
            cabinetData.ResetToInitialState();
        }
    }
    
    /// <summary>
    /// 同步柜子状态
    /// </summary>
    private void SyncCabinetItemState()
    {
        if (cabinetData == null || !gameObject.activeInHierarchy) return;
        
        // 如果柜子已经被打开，可以移除交互组件
        if (cabinetData.isOpened)
        {
            if (itemInteractable != null && itemInteractable.enabled)
            {
                itemInteractable.enabled = false;
            }
        }
        
    }
    
    /// <summary>
    /// 处理被动交互
    /// </summary>
    private void HandlePassiveInteraction()
    {
        if (cabinetData == null) return;
        
        // 如果已经显示过接近对话，不再显示
        if (cabinetData.hasShownApproachDialogue) return;
        
        ShowApproachDialogue();
        
        // 标记为已显示接近对话
        cabinetData.hasShownApproachDialogue = true;
        hasProcessedPassiveInteraction = true;
    }
    
    /// <summary>
    /// 显示接近时的对话
    /// </summary>
    private void ShowApproachDialogue()
    {
        if (cabinetData == null || string.IsNullOrEmpty(cabinetData.approachDialogue))
            return;
            
        // 获取对话管理器
        DialogueManager dialogueManager = Object.FindObjectOfType<DialogueManager>();
        if (dialogueManager == null)
            return;
            
        
        // 创建临时对话数据
        DialogueData dialogueData = ScriptableObject.CreateInstance<DialogueData>();
        
        // 创建对话节点
        DialogueNode node = new DialogueNode();
        node.nodeID = "cabinet_approach";
        node.text = cabinetData.approachDialogue;
        node.speakerID = cabinetData.playerNPCData.npcID;
        // 确保nextNodeID为空，表示对话结束
        node.nextNodeID = "";
        
        // 添加节点到对话数据
        dialogueData.AddNode(node);
        
        // 设置起始节点
        dialogueData.startNodeID = "cabinet_approach";
        
        
        
        
        // 设置时间缩放为1，确保对话正常进行
        Time.timeScale = 1f;
        
        // 开始对话
        dialogueManager.StartDialogue_Item(dialogueData);
        
    }
    
    /// <summary>
    /// 恢复原始对话数据的协程
    /// </summary>
    private IEnumerator RestoreOriginalDialogueData(NPCData npcData, DialogueData originalData)
    {
        // 等待对话完成
        yield return new WaitForSeconds(5f); // 粗略估计5秒完成对话
        
        
        // 确保交互系统状态正确
        ResetInteractionSystem();
    }
    
    /// <summary>
    /// 重置交互系统状态
    /// </summary>
    public void ResetInteractionSystem()
    {
        // 重置CabinetItemData状态
        if (cabinetData != null)
        {
            // 不要重置首次交互状态，那应该是持久性的状态变化
            // cabinetData.ResetCabinetState();
            
            // 同步柜子状态
            SyncCabinetItemState();
        }
        
        // 强制触发PlayerInteractionManager的状态重置
        var interactionManager = Object.FindObjectOfType<InteractionSystem.Core.PlayerInteractionManager>();
        if (interactionManager != null)
        {
            // 使用反射获取私有字段并重置
            var fields = new string[] { "isInteracting", "isInDialogue", "isInteractionPaused" };
            foreach (var fieldName in fields)
            {
                var field = interactionManager.GetType().GetField(fieldName, 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(interactionManager, false);
                }
            }
            
            // 通知PlayerInteractionManager检查可交互物体
            interactionManager.SendMessage("ResumeInteraction", null, SendMessageOptions.DontRequireReceiver);
            interactionManager.SendMessage("CheckForInteractable", null, SendMessageOptions.DontRequireReceiver);
        }
        
        // 确保时间缩放正确
        Time.timeScale = 1f;
        
    }
} 