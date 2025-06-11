using UnityEngine;
using NPCSystem.Core;
using NPCSystem.Dialogue;

namespace IneractionSystem.Core
{
    /// <summary>
    /// NPC交互组件，实现NPC的交互逻辑
    /// </summary>
    public class NPCInteractable : MonoBehaviour, INPCInteractable
    {
        [SerializeField] private NPCData npcData;
        [SerializeField] private bool canInteract = true;
        
        private DialogueManager dialogueManager;
        
        private void Start()
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
            
            if (npcData == null)
            {
                Debug.LogError("NPCInteractable: NPC数据为空");
            }
        }
        
        /// <summary>
        /// 获取交互提示文本
        /// </summary>
        public string GetInteractPrompt()
        {
            if (npcData == null) return "按E交谈";
            
            return npcData.interactPrompt;
        }
        
        /// <summary>
        /// 检查是否可以交互
        /// </summary>
        public bool CanInteract()
        {
            return canInteract && npcData != null;
        }
        
        /// <summary>
        /// 执行交互
        /// </summary>
        public void Interact()
        {
            if (!CanInteract()) return;
            
            // 开始对话
            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue(npcData.dialogueDatas[0], null);
            }
            else
            {
                Debug.LogWarning("NPCInteractable: 找不到对话管理器");
            }
        }
        
        /// <summary>
        /// 获取NPC的变换组件
        /// </summary>
        public Transform GetTransform()
        {
            return transform;
        }
        
        /// <summary>
        /// 获取NPC数据
        /// </summary>
        public NPCData GetNPCData()
        {
            return npcData;
        }
        
        /// <summary>
        /// 设置NPC数据
        /// </summary>
        public void SetNPCData(NPCData data)
        {
            npcData = data;
        }
        
        /// <summary>
        /// 设置是否可交互
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            canInteract = interactable;
        }
    }
}