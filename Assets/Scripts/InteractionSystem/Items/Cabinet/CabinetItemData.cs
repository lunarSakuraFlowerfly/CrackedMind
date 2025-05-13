using UnityEngine;
using System.Collections.Generic;
using InteractionSystem.Item;
using NPCSystem.Dialogue;
using NPCSystem.Core;
using System.Collections;
using NPCSystem.Story;
namespace InteractionSystem.Data
{
    /// <summary>
    /// 柜子物品数据，定义柜子的交互逻辑
    /// </summary>
    [CreateAssetMenu(fileName = "New Cabinet Item Data", menuName = "Items/Cabinet Item Data")]
    public class CabinetItemData : ItemData
    {
        [Header("钥匙设置")]
        public string requiredKeyID; // 需要的钥匙ID

        [Header("对话设置")]
        public string firstTimePrompt = "按E交互";
        public string lockedPrompt = "缺少钥匙";
        public string unlockPrompt = "使用钥匙开锁";
        public string firstTimeDescription = "柜子锁住了，需要钥匙";
        public string openDescription = "你打开了柜子，其中有一个东西放在里面";

        [Header("对话内容")]
        [TextArea(2, 3)]
        public string approachDialogue = "我好像把很重要的东西放到了里面。";
        [TextArea(2, 3)]
        public string[] afterFirstInteractionDialogues = new string[] {
            "好像以前把一些重要的东西放在这里...",
            "那把钥匙……应该在家里的某个地方，可是我已经记不清了。",
            "我……不确定自己是否真的想打开它，现在的我，或许还没准备好面对那些回忆。"
        };

        [Header("角色引用")]
        public NPCData playerNPCData; // 玩家角色的NPC数据，在Inspector中设置

        [Header("物品设置")]
        public ItemData itemToGive; // 打开柜子后给予的物品

        // 状态变量
        [HideInInspector]
        public bool isFirstTime = true;
        [HideInInspector]
        public bool isOpened = false;
        [HideInInspector]
        public bool hasShownApproachDialogue = false; // 是否已显示接近时的对话
        
        // 状态持久化
        private static Dictionary<string, bool> firstTimeStates = new Dictionary<string, bool>();
        private static Dictionary<string, bool> openedStates = new Dictionary<string, bool>();
        private static Dictionary<string, bool> approachDialogueStates = new Dictionary<string, bool>();

        // 游戏启动时重置
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetCabinetStates()
        {
            // 测试模式：强制重置所有柜子状态
            bool shouldClearAllStates = true; // 设置为true，确保每次启动游戏都重置柜子状态
            
            if (shouldClearAllStates)
            {
                // 清除内存中的状态
                firstTimeStates.Clear();
                openedStates.Clear();
                approachDialogueStates.Clear();
                
                // 清除PlayerPrefs中的柜子状态
                string[] keys = new string[] { "_FirstTime", "_Opened", "_ApproachDialogue" };
                string[] cabinetKeys = PlayerPrefs.GetString("CabinetKeys", "").Split(',');
                
                // 删除所有以Cabinet_开头的PlayerPrefs键
                foreach (string key in PlayerPrefs.GetString("CabinetKeys", "").Split(','))
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        foreach (var suffix in keys)
                        {
                            PlayerPrefs.DeleteKey($"Cabinet_{key}{suffix}");
                        }
                    }
                }
                
                // 额外的安全检查：清除所有可能的Cabinet_相关键
                // 遍历所有可能的键（这是一个粗略方法）
                foreach (var key in PlayerPrefs.GetString("AllKeys", "").Split(','))
                {
                    if (key.StartsWith("Cabinet_"))
                    {
                        PlayerPrefs.DeleteKey(key);
                    }
                }
                
                // 清除柜子ID列表
                PlayerPrefs.DeleteKey("CabinetKeys");
                PlayerPrefs.Save();
                
                Debug.Log("CabinetItemData: 所有柜子状态已重置为初始状态");
            }
            else
            {
                Debug.Log("CabinetItemData: 柜子状态保持不变");
            }
        }

        private void OnDialogueEnd()
        {
            //如果对话结束，且第一次执行为false
            Debug.Log("<color=blue>对话结束</color>");
            if(!isFirstTime)
            {
                StoryManager.Instance.CabinetFirstTouchComplete();
                Debug.Log("<color=green>柜子第一次交互完成</color>");
                //移除结束对话事件
                DialogueManager.Instance.OnDialogueEnd -= OnDialogueEnd;
            }
        }

        private void OnEnable()
        {
            // 先尝试从PlayerPrefs加载状态
            LoadItemState();
            
            // 如果PlayerPrefs没有数据，才从字典中恢复状态（向后兼容）
            if (!PlayerPrefs.HasKey($"Cabinet_{itemID}_FirstTime"))
            {
                if (firstTimeStates.TryGetValue(itemID, out bool firstTimeState))
                {
                    isFirstTime = firstTimeState;
                }
                else
                {
                    isFirstTime = true;
                }
            
                if (openedStates.TryGetValue(itemID, out bool openState))
                {
                    isOpened = openState;
                }
                else
                {
                    isOpened = false;
                }
            
                if (approachDialogueStates.TryGetValue(itemID, out bool approachState))
                {
                    hasShownApproachDialogue = approachState;
                }
                else
                {
                    hasShownApproachDialogue = false;
                }
                
                // 保存到PlayerPrefs
                SaveItemState();
            }
            
            Debug.Log($"CabinetItemData: OnEnable完成，当前状态 - itemID={itemID}, isFirstTime={isFirstTime}, isOpened={isOpened}");
        }

        public override string GetInteractPrompt()
        {
            // 已开启状态 - 无提示
            if (isOpened)
                return "";
                
            // 初次交互 - "按E交互"
            if (isFirstTime)
                return firstTimePrompt;
                
            // 有钥匙 - "使用钥匙开锁"
            if (HasKey())
                return unlockPrompt;
                
            // 默认 - "缺少钥匙"
            return lockedPrompt;
        }
        
        public override string GetDescription()
        {
            // 已开启或非初次且无钥匙 - 无描述
            if (isOpened || (!isFirstTime && !HasKey()))
                return "";
                
            // 初次交互 - "柜子锁住了，需要钥匙"
            if (isFirstTime)
            {
                return firstTimeDescription;
            }
                
                
            // 有钥匙 - "你打开了柜子，其中有一个东西放在里面"
            if (HasKey())
                return openDescription;
                
            return "";
        }

        public override bool CanInteract()
        {
                      
            // 已开启状态完全无法交互
            if (isOpened)
            {
                Debug.Log("柜子已开启，不可交互");
                return false;
            }
                
            // 不管是初次交互还是有钥匙，都允许交互
            if (isFirstTime || HasKey())
            {
                return true;
            }
            
            return true;
        }

        public override void OnInteract()
        {
            // 已开启状态，不做任何事
            if (isOpened)
            {
                Debug.Log("柜子已开启，不可交互");
                return;
            }
                
            // 非初次交互且无钥匙，只显示提示但不执行实际交互逻辑
            if (!isFirstTime && !HasKey())
            {
                // 重置交互状态，避免锁死
                ResetInteractionManagerState();
                return;
            }
            
            // 首次交互
            if (isFirstTime)
            {
                
                // 先保存状态再修改本地变量，确保持久化
                firstTimeStates[itemID] = false;
                isFirstTime = false;

                
                // 立即写入到PlayerPrefs，确保状态持久化
                SaveItemState();
                
                Debug.Log("首次查看柜子，了解到需要钥匙");
                
                // 显示首次交互后的对话
                ShowAfterFirstInteractionDialogues();
                return;
            }
            
            if (HasKey())
            {
                UseKeyAndOpenCabinet();
            }
        }

        // 修改为public
        public bool HasKey()
        {
            // 由于系统变更，暂时始终返回true用于调试
            Debug.LogWarning($"背包系统已更改，暂时允许使用钥匙[{requiredKeyID}]");
            return true; // 调试模式，始终返回true
        }

        private void UseKeyAndOpenCabinet()
        {
            // 使用钥匙并打开柜子
            // 由于系统变更，暂时跳过钥匙移除逻辑
            Debug.LogWarning($"背包系统已更改，跳过移除钥匙[{requiredKeyID}]的逻辑");
            
            // 更新物品描述，使其在GetDescription()中返回openDescription
            this.description = openDescription;
            
            // 显示开启描述
            ItemInfoUI itemInfoUI = Object.FindObjectOfType<ItemInfoUI>();
            if (itemInfoUI != null)
            {
                itemInfoUI.ShowItemInfo(this);
            }
            
            // 给予物品
            if (itemToGive != null)
            {
                // 由于系统变更，暂时跳过物品添加逻辑
                Debug.LogWarning($"背包系统已更改，跳过添加物品[{itemToGive.itemName}]的逻辑");
                Debug.Log($"获得物品: {itemToGive.itemName}");
            }
            
            // 标记柜子为已打开状态
            isOpened = true;
            openedStates[itemID] = true;
        }
        
        // 显示首次交互后的对话序列
        private void ShowAfterFirstInteractionDialogues()
        {
            //添加结束对话事件
            DialogueManager.Instance.OnDialogueEnd += OnDialogueEnd;
            
            if (afterFirstInteractionDialogues == null || afterFirstInteractionDialogues.Length == 0)
                return;
                
            // 获取对话管理器
            DialogueManager dialogueManager = Object.FindObjectOfType<DialogueManager>();
            if (dialogueManager == null)
                return;
                
       
            
            // 创建临时对话数据
            DialogueData dialogueData = CreateTempDialogueData();
            

            for (int i = 0; i < afterFirstInteractionDialogues.Length; i++)
            {
                DialogueNode node = new DialogueNode();
                node.nodeID = "cabinet_after_first_" + i;
                node.text = afterFirstInteractionDialogues[i];
                node.speakerID = playerNPCData.npcID; // 使用指定的角色ID
                
                // 如果不是最后一个节点，添加下一个节点的引用
                if (i < afterFirstInteractionDialogues.Length - 1)
                {
                    node.nextNodeID = "cabinet_after_first_" + (i + 1);
                }
                else
                {
                    // 对于最后一个节点，确保nextNodeID为空，表示对话结束
                    node.nextNodeID = null;
                }
                
                dialogueData.AddNode(node);
            }
            
            // 设置起始节点
            dialogueData.startNodeID = "cabinet_after_first_0";
            
            
            // 设置时间缩放为1，确保对话正常进行
            Time.timeScale = 1f;
            
            // 再次确认isFirstTime状态已更新
            if (firstTimeStates.ContainsKey(itemID))
            {
                Debug.Log($"CabinetItemData: 显示对话前再次确认isFirstTime={firstTimeStates[itemID]}");
            }
            
            // 开始对话
            dialogueManager.StartDialogue_Item(dialogueData);
            
        }
        
        /// <summary>
        /// 重置柜子状态，在被动交互后可手动调用此方法
        /// </summary>
        public void ResetCabinetState()
        {
            // 如果已开启，不进行重置
            if (isOpened)
                return;
                
            // 重置持久化状态
            if (firstTimeStates.ContainsKey(itemID))
            {
                // 重置为初次交互状态，方便调试
                firstTimeStates[itemID] = true;
                isFirstTime = true;
                Debug.Log($"CabinetItemData: 重置柜子 {itemID} 为初次交互状态");
            }
            
            // 通知交互管理器刷新状态
            var interactionManager = Object.FindObjectOfType<InteractionSystem.Core.PlayerInteractionManager>();
            if (interactionManager != null)
            {
                // 强制所有状态重置
                var fields = new string[] { "isInteracting", "isInDialogue", "isInteractionPaused" };
                foreach (var fieldName in fields)
                {
                    var field = interactionManager.GetType().GetField(fieldName, 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (field != null)
                    {
                        field.SetValue(interactionManager, false);
                        Debug.Log($"CabinetItemData: 强制重置PlayerInteractionManager的{fieldName}状态");
                    }
                }
                
                // 请求交互管理器检查可交互物体
                interactionManager.SendMessage("CheckForInteractable", null, SendMessageOptions.DontRequireReceiver);
            }
        }
        
        /// <summary>
        /// 将柜子完全重置到初始状态（无论是否已开启）
        /// </summary>
        public void ResetToInitialState()
        {
            // 重置所有状态变量
            isFirstTime = true;
            isOpened = false;
            hasShownApproachDialogue = false;
            
            // 更新内存中的字典
            if (firstTimeStates.ContainsKey(itemID))
            {
                firstTimeStates[itemID] = true;
            }
            else
            {
                firstTimeStates.Add(itemID, true);
            }
            
            if (openedStates.ContainsKey(itemID))
            {
                openedStates[itemID] = false;
            }
            else
            {
                openedStates.Add(itemID, false);
            }
            
            if (approachDialogueStates.ContainsKey(itemID))
            {
                approachDialogueStates[itemID] = false;
            }
            else
            {
                approachDialogueStates.Add(itemID, false);
            }
            
            // 更新PlayerPrefs
            PlayerPrefs.SetInt($"Cabinet_{itemID}_FirstTime", 1);
            PlayerPrefs.SetInt($"Cabinet_{itemID}_Opened", 0);
            PlayerPrefs.SetInt($"Cabinet_{itemID}_ApproachDialogue", 0);
            PlayerPrefs.Save();
            
            Debug.Log($"CabinetItemData: 柜子 {itemID} 已完全重置到初始状态");
            
            // 重置交互管理器状态
            ResetInteractionManagerState();
        }
        
        // 恢复原始对话数据的协程
        private IEnumerator RestoreOriginalDialogueData(NPCData npcData, DialogueData originalData)
        {
            // 等待对话完成（粗略估计5秒）
            yield return new WaitForSeconds(5f);
            
    
            
            // 通知PlayerInteractionManager对话已结束
            var interactionManager = Object.FindObjectOfType<InteractionSystem.Core.PlayerInteractionManager>();
            if (interactionManager != null)
            {
                // 强制设置交互状态为False，确保可以进行后续交互
                var field = interactionManager.GetType().GetField("isInteracting", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(interactionManager, false);
                }
                
                // 重置对话状态标志
                var dialogueField = interactionManager.GetType().GetField("isInDialogue", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (dialogueField != null)
                {
                    dialogueField.SetValue(interactionManager, false);
                }
                
                // 如果有恢复交互的方法，调用它
                interactionManager.SendMessage("ResumeInteraction", null, SendMessageOptions.DontRequireReceiver);
                
                // 重置交互暂停
                var pauseField = interactionManager.GetType().GetField("isInteractionPaused", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (pauseField != null)
                {
                    pauseField.SetValue(interactionManager, false);
                }
                
                // 立即检查可交互物体
                interactionManager.SendMessage("CheckForInteractable", null, SendMessageOptions.DontRequireReceiver);
                
                // 强制设置Time.timeScale为1
                Time.timeScale = 1f;
            }
        }
        
        // 创建临时对话数据
        private DialogueData CreateTempDialogueData()
        {
            DialogueData dialogueData = ScriptableObject.CreateInstance<DialogueData>();
            return dialogueData;
        }

        // 重置交互管理器状态
        private void ResetInteractionManagerState()
        {
            // 获取交互管理器
            var interactionManager = Object.FindObjectOfType<InteractionSystem.Core.PlayerInteractionManager>();
            if (interactionManager != null)
            {
                // 使用反射访问私有字段
                var field = interactionManager.GetType().GetField("isInteracting", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                if (field != null)
                {
                    field.SetValue(interactionManager, false);
                }
                
                // 如果有直接重置所有标志的方法，调用它
                interactionManager.SendMessage("ResetInteractionFlags", null, SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <summary>
        /// 保存物品状态到PlayerPrefs
        /// </summary>
        private void SaveItemState()
        {
            // 保存首次交互状态
            PlayerPrefs.SetInt($"Cabinet_{itemID}_FirstTime", isFirstTime ? 1 : 0);
            // 保存开启状态
            PlayerPrefs.SetInt($"Cabinet_{itemID}_Opened", isOpened ? 1 : 0);
            // 保存已显示接近对话状态
            PlayerPrefs.SetInt($"Cabinet_{itemID}_ApproachDialogue", hasShownApproachDialogue ? 1 : 0);
            
            // 确保立即写入
            PlayerPrefs.Save();

        }
        
        /// <summary>
        /// 加载物品状态
        /// </summary>
        private void LoadItemState()
        {
            // 尝试从PlayerPrefs加载状态
            if (PlayerPrefs.HasKey($"Cabinet_{itemID}_FirstTime"))
            {
                isFirstTime = PlayerPrefs.GetInt($"Cabinet_{itemID}_FirstTime") == 1;
                if (firstTimeStates.ContainsKey(itemID))
                {
                    firstTimeStates[itemID] = isFirstTime;
                }
                else
                {
                    firstTimeStates.Add(itemID, isFirstTime);
                }
            }
            
            if (PlayerPrefs.HasKey($"Cabinet_{itemID}_Opened"))
            {
                isOpened = PlayerPrefs.GetInt($"Cabinet_{itemID}_Opened") == 1;
                if (openedStates.ContainsKey(itemID))
                {
                    openedStates[itemID] = isOpened;
                }
                else
                {
                    openedStates.Add(itemID, isOpened);
                }
            }
            
            if (PlayerPrefs.HasKey($"Cabinet_{itemID}_ApproachDialogue"))
            {
                hasShownApproachDialogue = PlayerPrefs.GetInt($"Cabinet_{itemID}_ApproachDialogue") == 1;
                if (approachDialogueStates.ContainsKey(itemID))
                {
                    approachDialogueStates[itemID] = hasShownApproachDialogue;
                }
                else
                {
                    approachDialogueStates.Add(itemID, hasShownApproachDialogue);
                }
            }
            
        }
    }
}