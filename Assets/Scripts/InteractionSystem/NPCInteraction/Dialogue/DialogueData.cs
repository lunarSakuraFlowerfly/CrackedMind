using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace NPCSystem.Dialogue
{
    /// <summary>
    /// 对话者类型
    /// </summary>
    public enum SpeakerType
    {
        NPC,
        Player,
        Other
    }

    /// <summary>
    /// 对话者数据
    /// </summary>
    [System.Serializable]
    public class SpeakerData
    {
        public string speakerID;
        public string speakerName;
        public SpeakerType speakerType;
        public Sprite portrait;
    }

    /// <summary>
    /// 对话数据，定义一组对话节点
    /// </summary>
    [CreateAssetMenu(fileName = "New Dialogue Data", menuName = "NPCs/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        [Header("对话ID")]
        public string dialogueID = "001";

        [Header("对话设置")]
        public string startNodeID;
        public List<SpeakerData> speakers = new List<SpeakerData>();
        
        [Header("对话节点")]
        [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();
        
        // 用于跟踪节点数量变化
        [HideInInspector]
        public int lastNodeCount = 0;
        
        public List<DialogueNode> Nodes => nodes;
        
        public void AddNode(DialogueNode node)
        {
            nodes.Add(node);
        }
        
        public DialogueNode GetNode(string nodeID)
        {
            return nodes.Find(n => n.nodeID == nodeID);
        }
        
        public SpeakerData GetSpeaker(string speakerID)
        {
            return speakers.Find(s => s.speakerID == speakerID);
        }
        
        /// <summary>
        /// 判断节点是否为终止节点
        /// </summary>
        /// <param name="nodeID">节点ID</param>
        /// <returns>如果节点没有下一个节点ID且没有选项，则返回true</returns>
        public bool IsEndNode(string nodeID)
        {
            DialogueNode node = GetNode(nodeID);
            if (node == null) return true;
            
            // 如果没有下一个节点ID且没有选项，则为终止节点
            return string.IsNullOrEmpty(node.nextNodeID) && (node.choices == null || node.choices.Count == 0);
        }
        
        private void OnValidate()
        {
            // 确保至少有一个说话者
            if (speakers.Count == 0)
            {
                speakers.Add(new SpeakerData
                {
                    speakerID = "npc_001",
                    speakerName = "默认NPC",
                    speakerType = SpeakerType.NPC
                });
            }
            
            // 检查节点变化
            if (nodes.Count != lastNodeCount)
            {
                // 只为新添加的节点或空节点分配唯一id
                for (int i = 0; i < nodes.Count; i++)
                {
                    // 只有当节点为null或者是新添加的节点且没有nodeID时才创建新节点
                    if (nodes[i] == null || 
                        (i >= lastNodeCount && nodes.Count > lastNodeCount && string.IsNullOrEmpty(nodes[i].nodeID)))
                    {
                        //生成唯一id
                        string nodeID = "node_" + System.Guid.NewGuid().ToString().Substring(0, 8);

                        //创建新节点
                        nodes[i] = new DialogueNode
                        {
                            nodeID = nodeID,
                            speakerID = speakers.Count > 0 ? speakers[0].speakerID : "",
                            text = "",
                            nextNodeID = "",
                            choices = new List<DialogueChoice>(),
                        };
                    }
                }
                
                // 如果是第一个节点，设置为起始节点
                if (nodes.Count > 0 && string.IsNullOrEmpty(startNodeID))
                {
                    startNodeID = nodes[0].nodeID;
                }

                //如果删除了起始节点，更新起始节点
                bool startNodeExists = nodes.Exists(n => n.nodeID == startNodeID);
                if(!startNodeExists && nodes.Count>0)
                {
                    startNodeID = nodes[0].nodeID;
                }
            }
            
            // 更新节点计数
            lastNodeCount = nodes.Count;
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(DialogueData))]
    public class DialogueDataEditor : Editor
    {
        private Vector2 scrollPosition;
        
        public override void OnInspectorGUI()
        {
            DialogueData dialogueData = (DialogueData)target;

            // 绘制对话ID
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("对话ID", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            dialogueData.dialogueID = EditorGUILayout.TextField("对话ID", dialogueData.dialogueID);
            
            // 绘制基本设置
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("对话设置", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            dialogueData.startNodeID = EditorGUILayout.TextField("起始节点ID", dialogueData.startNodeID);
            
            // 绘制说话者列表
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("说话者列表", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            
            SerializedProperty speakersProperty = serializedObject.FindProperty("speakers");
            EditorGUILayout.PropertyField(speakersProperty, true);
            
            // 绘制节点列表
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("对话节点", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            //添加自定义按钮
            if(GUILayout.Button("添加新对话节点",GUILayout.Height(30)))
            {
                // 使用AddNode方法添加新节点
                Undo.RecordObject(dialogueData, "Add Dialogue Node");
                DialogueNode newNode = new DialogueNode();
                dialogueData.AddNode(newNode);
                EditorUtility.SetDirty(dialogueData);
                serializedObject.Update();
                Repaint();
            }
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300));
            SerializedProperty nodesProperty = serializedObject.FindProperty("nodes");
            EditorGUILayout.PropertyField(nodesProperty, true);
            EditorGUILayout.EndScrollView();
            
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}