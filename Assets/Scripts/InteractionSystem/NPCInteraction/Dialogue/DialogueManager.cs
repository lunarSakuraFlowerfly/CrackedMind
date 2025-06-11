using UnityEngine;
using System.Collections.Generic;
using NPCSystem.Core;
using NPCSystem.UI;
using NPCSystem.Behavior;
using System;
using InteractionSystem.Core;
using UnityEngine.UI;

namespace NPCSystem.Dialogue
{
    public delegate void DialogueEndedCallback(string endNodeID);
    /// <summary>
    /// 对话管理器，管理对话的进行
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {

        #region Singleton
        public static DialogueManager Instance;
        private void Awake()
        {
            if(Instance==null)
            {
                Instance=this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region 服务于故事系统

        #endregion


        #region Events
        //对话开始事件
       public event Action<NPCData> OnDialogueStart;
       //对话结束事件
       public event Action OnDialogueEnd;
       // 对话选项选择事件
       public event Action<string> OnDialogueChoiceSelected;

       //添加对话结束回调字段
       private DialogueEndedCallback m_CurrentCallback;
       #endregion

        [SerializeField] private DialogueUI dialogueUI;
        
        private DialogueData currentDialogue;
        private DialogueNode currentNode;
        private NPCData currentNPC;
        private string m_CurrentNodeID;
        
        private bool isDialogueActive = false;
        
        private void Start()
        {
            if (dialogueUI == null)
            {
                dialogueUI = FindObjectOfType<DialogueUI>();
            }

        }
        


        
        /// <summary>
        /// 开始对话
        /// </summary>
        public void StartDialogue(DialogueData dialogueData,DialogueEndedCallback callback = null)
        {
   
            
            if (dialogueUI == null)
            {
                Debug.LogError("DialogueUI reference is missing in DialogueManager!");
                dialogueUI = FindObjectOfType<DialogueUI>();
                if (dialogueUI == null)
                {
                    Debug.LogError("Could not find DialogueUI in the scene!");
                    return;
                }
            }
            currentDialogue = dialogueData;
            m_CurrentCallback = callback;

           // 先设置对话界面为活动状态，再开始对话
           SetDialogueUIActive(true);
           // 开始对话 
           StartFromNode(dialogueData.startNodeID);
        }
        
        /// <summary>
        /// 开始对话 - 兼容原始交互系统
        /// </summary>
        public void StartDialogue_Item(DialogueData dialogueData)
        {    
            // 调用新的开始对话方法
            StartDialogue(dialogueData, null);
        }
        
        /// <summary>
        /// 结束对话
        /// </summary>
        public void EndDialogue()
        {
           //保存当前节点ID以供回调使用
           string endNodeID = currentNode != null ? currentNode.nodeID : string.Empty;

           //清理对话状态
           currentDialogue = null;
           currentNode = null;
           m_CurrentNodeID = string.Empty;

           //关闭对话界面
           SetDialogueUIActive(false);

           //触发对话结束回调
           if(m_CurrentCallback != null)
           {
            m_CurrentCallback(endNodeID);
            m_CurrentCallback = null;
           }

           //触发对话结束事件
           OnDialogueEnd?.Invoke();


        }
        
        /// <summary>
        /// 移动到指定节点
        /// </summary>
        public void MoveToNode(string nodeID)
        {
            if (currentDialogue == null) return;
            
            // 查找节点
            DialogueNode node = currentDialogue.GetNode(nodeID);
            
            if (node != null)
            {
                currentNode = node;
                ProcessCurrentNode();
            }
            else
            {
                Debug.LogWarning($"找不到对话节点: {nodeID}");
                
                EndDialogue();
            }
        }
        
        /// <summary>
        /// 从指定节点开始对话
        /// </summary>
        private void StartFromNode(string nodeID)
        {
            // 记录当前节点ID
            m_CurrentNodeID = nodeID;
            
            // 移动到开始节点
            MoveToNode(nodeID);
        }
        
        /// <summary>
        /// 处理当前节点
        /// </summary>
        private void ProcessCurrentNode()
        {
            if (currentNode == null)
            {
                Debug.LogError("DialogueManager: 当前节点为空，无法处理");
                EndDialogue();
                return;
            }
            
            if (currentDialogue == null)
            {
                Debug.LogError("DialogueManager: 当前对话为空，无法处理");
                EndDialogue();
                return;
            }
            
            if (dialogueUI == null)
            {
                Debug.LogError("DialogueManager: DialogueUI为空，无法处理");
                EndDialogue();
                return;
            }
            
            // 获取说话者信息
            SpeakerData speaker = currentDialogue.GetSpeaker(currentNode.speakerID);
            if(speaker == null)
            {
                Debug.LogWarning($"DialogueManager: 未找到说话者 ID: {currentNode.speakerID}，使用默认说话者");
            }
            
            // 更新对话 - 使用新API
            dialogueUI.SetDialogueNode(currentNode);
            dialogueUI.SetDialogue(currentNode.text, 
                                   speaker?.speakerName ?? "未知", 
                                   speaker?.portrait,
                                   speaker?.speakerType ?? SpeakerType.NPC);
            
            // 获取有效的选择
            List<DialogueChoice> validChoices = GetValidChoices(currentNode.choices);
            
            // 显示选择 - 使用新API
            if (validChoices.Count > 0)
            {
                List<string> choiceTexts = new List<string>();
                foreach (var choice in validChoices)
                {
                    choiceTexts.Add(choice.text);
                }
                dialogueUI.SetChoices(choiceTexts);
            }
            else
            {
                dialogueUI.ClearChoices();
            }
        }
        
        /// <summary>
        /// 获取有效的选择
        /// </summary>
        private List<DialogueChoice> GetValidChoices(List<DialogueChoice> choices)
        {
            List<DialogueChoice> validChoices = new List<DialogueChoice>();
            
            if (choices == null) return validChoices;
            
            foreach (var choice in choices)
            {
                bool isValid = true;
                
                // 检查条件
                if (choice.conditions != null && choice.conditions.Count > 0)
                {
                    foreach (var condition in choice.conditions)
                    {
                        if (!CheckCondition(condition))
                        {
                            isValid = false;
                            break;
                        }
                    }
                }
                
                if (isValid)
                {
                    validChoices.Add(choice);
                }
            }
            
            return validChoices;
        }
        
        /// <summary>
        /// 检查条件
        /// </summary>
        private bool CheckCondition(DialogueCondition condition)
        {
            // 这里实现条件检查逻辑
            // 例如检查玩家是否有物品、是否完成任务等
            
            return true; // 默认返回true，实际应根据条件判断
        }
        
        /// <summary>
        /// 选择对话选项
        /// </summary>
        public void ChooseOption(DialogueChoice choice)
        {
            if (choice == null) return;
            
            
            // 触发选项选择事件
            OnDialogueChoiceSelected?.Invoke(choice.text);
            
            // 移动到下一个节点
            if (!string.IsNullOrEmpty(choice.nextNodeID))
            {
                MoveToNode(choice.nextNodeID);
            }
            else
            {
                EndDialogue();
            }
        }
        
        /// <summary>
        /// 选择选项
        /// </summary>
        public void SelectChoice(int choiceIndex)
        {
            if (currentNode == null || currentNode.choices == null) return;
            
            if (choiceIndex >= 0 && choiceIndex < currentNode.choices.Count)
            {
                ChooseOption(currentNode.choices[choiceIndex]);
            }
        }
        
        /// <summary>
        /// 继续对话
        /// </summary>
        public void ContinueDialogue()
        {
            if (currentNode == null) return;
            

            // 移动到下一个节点或结束对话
            if (!string.IsNullOrEmpty(currentNode.nextNodeID))
            {
                MoveToNode(currentNode.nextNodeID);
            }
            else
            {
                // 确保在结束对话前已经执行了所有动作
                EndDialogue();
            }
        }
        
       
   
        
        /// <summary>
        /// 检查对话是否激活
        /// </summary>
        public bool IsDialogueActive()
        {
            return isDialogueActive;
        }

        /// <summary>
        /// 设置对话UI活动状态
        /// </summary>
        private void SetDialogueUIActive(bool active)
        {
           if(dialogueUI != null)
           {
                // 激活对象
                dialogueUI.gameObject.SetActive(active);
                isDialogueActive = active;
                
                // 确保UI已经完全激活
                if(active)
                {
                    // 强制更新UI布局
                    Canvas.ForceUpdateCanvases();
                }
           }
           else
           {
                Debug.LogWarning("DialogueUI为空，无法设置活动状态");
           }
        }
        
    }
}