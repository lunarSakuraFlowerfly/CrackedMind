using UnityEngine;
using NPCSystem.Dialogue;
using NPCSystem.UI;

namespace NPCSystem.Core
{
    /// <summary>
    /// NPC交互管理器，管理玩家与NPC的交互
    /// </summary>
    public class NPCInteractionManager : MonoBehaviour
    {
        [Header("交互设置")]
        [SerializeField] private float interactionRange = 3f;
        [SerializeField] private LayerMask npcLayer;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        
        [Header("UI引用")]
        [SerializeField] private NPCPromptUI promptUI;
        
        [Header("对话管理器")]
        [SerializeField] private DialogueManager dialogueManager;
        
        private INPCInteractable currentNPC;
        private bool isInteracting = false;
        
        private void Start()
        {
            // 查找提示UI
            if (promptUI == null)
            {
                promptUI = FindObjectOfType<NPCPromptUI>();
            }
            
            // 查找对话管理器
            if (dialogueManager == null)
            {
                dialogueManager = FindObjectOfType<DialogueManager>();
            }
        }
        
        private void Update()
        {
            // 如果正在对话，不检测交互
            if (dialogueManager != null && dialogueManager.IsDialogueActive())
            {
                isInteracting = true;
                return;
            }
            else
            {
                isInteracting = false;
            }
            
            // 检测可交互NPC
            CheckForInteractableNPC();
            
            // 处理交互输入
            if (currentNPC != null && Input.GetKeyDown(interactKey))
            {
                currentNPC.Interact();
                isInteracting = true;
            }
        }
        
        /// <summary>
        /// 检测可交互NPC
        /// </summary>
        private void CheckForInteractableNPC()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange, npcLayer);
            
            INPCInteractable closestNPC = null;
            float closestDistance = float.MaxValue;
            
            foreach (var collider in colliders)
            {
                INPCInteractable npc = collider.GetComponent<INPCInteractable>();
                
                if (npc != null && npc.CanInteract())
                {
                    float distance = Vector2.Distance(transform.position, collider.transform.position);
                    
                    if (distance < closestDistance)
                    {
                        closestNPC = npc;
                        closestDistance = distance;
                    }
                }
            }
            
            // 如果找到了新的可交互NPC，或者当前可交互NPC变了
            if (closestNPC != currentNPC)
            {
                // 更新当前可交互NPC
                currentNPC = closestNPC;
                
                // 更新提示UI
                UpdatePromptUI();
            }
        }
        
        /// <summary>
        /// 更新提示UI
        /// </summary>
        private void UpdatePromptUI()
        {
            if (promptUI == null) return;
            
            if (currentNPC != null)
            {
                promptUI.Show(currentNPC.GetInteractPrompt(), currentNPC.GetTransform());
            }
            else
            {
                promptUI.Hide();
            }
        }
        
        /// <summary>
        /// 在场景视图中显示交互范围
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}