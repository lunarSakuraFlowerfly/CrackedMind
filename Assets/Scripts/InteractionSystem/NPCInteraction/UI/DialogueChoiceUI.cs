using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NPCSystem.Dialogue;

namespace NPCSystem.UI
{
    /// <summary>
    /// 对话选择UI，显示单个对话选项
    /// </summary>
    public class DialogueChoiceUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI choiceText;
        [SerializeField] private Button choiceButton;
        
        private DialogueChoice dialogueChoice;
        
        /// <summary>
        /// 设置对话选择
        /// </summary>
        public void SetChoice(DialogueChoice choice, System.Action<DialogueChoice> onChoiceSelected)
        {
            dialogueChoice = choice;
            
            if (choiceText != null && choice != null)
            {
                choiceText.text = choice.text;
            }
            
            if (choiceButton != null)
            {
                choiceButton.onClick.RemoveAllListeners();
                choiceButton.onClick.AddListener(() => onChoiceSelected?.Invoke(choice));
            }
        }
    }
}