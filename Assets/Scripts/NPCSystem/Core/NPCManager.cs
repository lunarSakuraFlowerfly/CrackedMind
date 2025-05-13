using System.Collections.Generic;
using UnityEngine;
namespace NPCSystem.Core
{
    
    /// <summary>
    /// NPC总管理器，负责创建和管理所有NPC实体
    /// </summary>
    public class NPCManager : MonoBehaviour, INPCManager
    {
        #region 私有字段
        private List<NPCEntity> m_NPCs = new List<NPCEntity>();
        private Dictionary<string, INPCEntity> m_ActiveNPCs = new Dictionary<string, INPCEntity>();
        #endregion
        #region Singleton
        public static NPCManager Instance { get; private set; }

        
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            //获取场景中Layer为Interactable的物体
            GameObject[] interactableObjects = GameObject.FindGameObjectsWithTag("NPCs");
            foreach (GameObject obj in interactableObjects)
            {
      
                    m_NPCs.Add(obj.GetComponent<NPCEntity>());
            }
            //将场景中的Npcs添加到m_ActiveNPCs字典
            foreach (NPCEntity npc in m_NPCs)
            {
                m_ActiveNPCs[npc.Data.npcID] = npc;
            }
        }
        #endregion


      
        #region 公共方法
        /// <summary>
        /// 根据ID获取NPC实体
        /// </summary>
        public INPCEntity GetNPC(string npcID)
        {
            if (m_ActiveNPCs.TryGetValue(npcID, out INPCEntity npc))
            {
                return npc;
            }
            return null;
        }

        /// <summary>
        /// 生成NPC到场景
        /// </summary>
        public INPCEntity SpawnNPC(string npcID, Transform spawnPoint)
        {
            // 查找预制体
            NPCEntity prefab = m_NPCs.Find(p => p.Data.npcID == npcID);
            if (prefab == null)
            {
                Debug.LogWarning($"未找到NPC预制体: {npcID}");
                return null;
            }
            
            // 如果NPC已存在，直接返回
            if (m_ActiveNPCs.TryGetValue(npcID, out INPCEntity existingNPC))
            {
                return existingNPC;
            }
            
            // 实例化NPC
            NPCEntity npcInstance = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            INPCEntity npcEntity = npcInstance;
            
            // 添加到活动NPC字典
            m_ActiveNPCs[npcID] = npcEntity;
            
            return npcEntity;
        }

        /// <summary>
        /// 从场景移除NPC
        /// </summary>
        public void DespawnNPC(string npcID)
        {
            if (m_ActiveNPCs.TryGetValue(npcID, out INPCEntity npc))
            {
                // 尝试获取GameObject并销毁
                if (npc is MonoBehaviour npcBehaviour)
                {
                    Destroy(npcBehaviour.gameObject);
                }
                
                // 从活动NPC字典中移除
                m_ActiveNPCs.Remove(npcID);
            }
        }
        
        /// <summary>
        /// 获取场景中的所有NPC
        /// </summary>
        public List<INPCEntity> GetAllNPCs()
        {
            return new List<INPCEntity>(m_ActiveNPCs.Values);
        }

        /// <summary>
        /// 更新所有NPC
        /// </summary>
        public void UpdateAllNPCs()
        {
            // 由Unity的Update循环自动处理
        }
        #endregion
    }
}   