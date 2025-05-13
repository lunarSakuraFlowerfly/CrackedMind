using UnityEngine;
using System.Collections.Generic;

namespace NPCSystem.Dialogue
{
    /// <summary>
    /// 对话选择，定义玩家可以选择的对话选项
    /// </summary>
    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceID;
        public string text;
        public string nextNodeID;
        public List<DialogueCondition> conditions = new List<DialogueCondition>(); // 显示此选项的条件

    }
}