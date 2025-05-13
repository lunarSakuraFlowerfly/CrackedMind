using UnityEngine;

using NPCSystem.Dialogue;
using NPCSystem.Behavior;


namespace NPCSystem.Core
{

    /// <summary>
    /// NPC实体基类，所有NPC的基本组件
    /// </summary>
    public class NPCEntity : MonoBehaviour, INPCEntity, INPCInteractable
    {
        #region 私有字段
       
        [SerializeField] private NPCData m_NPCData;

        //NPC移动组件
        private NPCMovement m_NPCMovement;

        //添加对话相关字段
        [Header("对话相关")]
        private DialogueData m_DeafaultDialogue;
        [SerializeField] private string m_InteractPrompt ="按E对话";
        [SerializeField] private bool m_CanInteract = true;
        

        //对话相关
        private DialogueManager m_DialogueManager;

        #endregion

        #region 公共属性
        public NPCData Data => m_NPCData;
        #endregion

        #region Unity生命周期
        private void Awake()
        {

            m_DialogueManager=FindObjectOfType<DialogueManager>();
            m_NPCMovement=GetComponent<NPCMovement>();
            
        }

        private void Start()
        {
            m_DeafaultDialogue=m_NPCData.dialogueDatas[0];

            //初始化NPC移动组件
            m_NPCMovement=GetComponent<NPCMovement>();
            if(m_NPCMovement==null)
            {
                m_NPCMovement=gameObject.AddComponent<NPCMovement>();
            }
        }

        private void Update()
        {

        }
        #endregion

        #region INPCEntity接口实现
        /// <summary>
        /// 执行特定行为
        /// </summary>
        public void ExecuteBehavior(string behaviorID)
        {
            // 实现执行"执行行为: 行为逻辑
            Debug.Log($"NPC {m_NPCData.npcID} 执行行为: {behaviorID}");
        }


        /// <summary>
        /// 与该NPC交互
        /// </summary>
        public void Interact(GameObject initiator)
        {
            // 默认触发对话
            TriggerDefaultDialogue();
        }
        
        /// <summary>
        /// 获取NPC位置
        /// </summary>
        public Vector3 GetPosition()
        {
            return transform.position;
        }
        
        /// <summary>
        /// 获取NPC变换组件
        /// </summary>
        public Transform GetTransform()
        {
            return transform;
        }
        #endregion

        #region INPCInteractable接口实现
        /// <summary>
        /// 获取交互提示文本
        /// </summary>
        public string GetInteractPrompt()
        {
            return m_InteractPrompt;
        }

        /// <summary>
        /// 是否可以交互
        /// </summary>
        public bool CanInteract()
        {
            //根据NPC状态判断是否交互
            if(!m_CanInteract) return false;


            //可以添加更多条件判断，例如基于剧情进度
            return true;
        }

        /// <summary>
        ///执行交互
        /// </summary>
        public void Interact()
        {

            //触发默认对话
            TriggerDefaultDialogue();
        }

        /// <summary>
        /// 获取NPC数据
        /// </summary>
        public NPCData GetNPCData()
        {
            return m_NPCData;
        }
        #endregion

        #region 对话系统相关方法
        /// <summary>
        /// 触发默认对话
        /// </summary>
        private void TriggerDefaultDialogue()
        {
            TriggerDialogue(m_DeafaultDialogue);
        }

        /// <summary>
        /// 触发特定对话
        /// </summary>
        public void TriggerDialogue(DialogueData dialogueData)
        {
            if(m_DialogueManager==null || dialogueData==null)
            {
                Debug.LogWarning($"NPC {m_NPCData.npcID} 触发对话失败，对话管理器或对话数据为空");
                return;
            }
            // 开始对话，并注册对话结束回调
            m_DialogueManager.StartDialogue(dialogueData,OnDialogueEnded);
        }

        /// <summary>
        /// 根据对话ID触发对话
        /// </summary>
        public void TriggerDialogueByID(string dialogueID)
        {
            if(Data == null)
            {
                Debug.LogError($"NPC Data没有进行设置");
                return;
            }
            //从NPCData中找到对应的对话数据
            DialogueData dialogueData=m_NPCData.dialogueDatas.Find(data=>data.dialogueID==dialogueID);
            if(dialogueData==null)
            {
                Debug.LogError($"无法找到对话数据：{dialogueID}。NPC:{m_NPCData.npcID}");
                return;
            }
            TriggerDialogue(dialogueData);
        } 
        
         /// <summary>
        /// 对话结束回调
        /// </summary>
        private void OnDialogueEnded(string endNodeID)
        {
            // 处理对话结束逻辑
            // 例如，根据结束节点ID执行不同动作
            
            

        }

        /// <summary>
        /// 加载对话数据（示例实现）
        /// </summary>
        private DialogueData LoadDialogueData(string dialogueID)
        {
            // 实际项目中应该从资源管理系统加载
            // 这里仅作为示例返回默认对话
            return m_DeafaultDialogue;
        }

        /// <summary>
        /// 移动到指定位置
        /// </summary>
        public void MoveTo(Vector3 position)
        {
            //实现NPC移动到指定位置
            if(m_NPCMovement!=null)
            {
                m_NPCMovement.MoveTo(position);
            }
        }

        /// <summary>
        /// 朝向玩家
        /// </summary>
        public void LookAt(Vector3 targetPosition)
        {
            if(m_NPCMovement!=null)
            {
                StartCoroutine(m_NPCMovement.LookAt(targetPosition));
            }
        }

        /// <summary>
        /// 设置坐下状态
        /// </summary>
        public void SetSitState(bool isSit,Vector3 targetPosition)
        {
            if(m_NPCMovement!=null)
            {
                m_NPCMovement.SetSitState(isSit);
                transform.position = targetPosition;
            }
        }
        
        #endregion

    }
}