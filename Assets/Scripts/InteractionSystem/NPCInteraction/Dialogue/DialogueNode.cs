using UnityEngine;
using System.Collections.Generic;

namespace NPCSystem.Dialogue
{
    /// <summary>
    /// 对话节点，定义对话的一个部分
    /// </summary>
    [System.Serializable]
    public class DialogueNode
    {
        public string nodeID;
        public string speakerID;  // 引用DialogueData中的SpeakerData
        [TextArea(3, 10)]
        public string text;
        public string nextNodeID; // 如果没有选择，直接跳转到下一个节点
        public List<DialogueChoice> choices = new List<DialogueChoice>();
        public AudioClip voiceClip; // 语音
        
        // 用于跟踪选项数量变化，在Inspector中隐藏
        [HideInInspector]
        public int lastChoiceCount = 0;
    }
}