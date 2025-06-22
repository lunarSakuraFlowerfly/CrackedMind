using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using NPCSystem.Dialogue;
using NPCSystem.Core;

namespace NPCSystem.UI
{
    /// <summary>
    /// 对话UI组件，处理对话的显示和选项选择
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        [Header("对话框设置")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Image speakerImage;
        [SerializeField] private GameObject continueIcon;
        
        [Header("选项设置")]
        [SerializeField] private GameObject choicesPanel;
        [SerializeField] private GameObject choiceButtonPrefab;
        [SerializeField] private Transform choiceButtonsContainer;
        
        [Header("打字机效果")]
        [SerializeField] private bool useTypewriterEffect = true;
        [SerializeField] private float typingSpeed = 0.05f;
        [SerializeField] private AudioClip typingSound;
        [SerializeField] private float typingSoundVolume = 0.5f;
        
        [Header("交互设置")]
        [SerializeField] private KeyCode continueKey = KeyCode.Return;
        [SerializeField] private bool autoCloseAfterDelay = false;
        [SerializeField] private float autoCloseDelay = 5f;
        
        private bool isTyping = false;
        private bool isDialogueComplete = false;
        private Coroutine typingCoroutine;
        private Coroutine autoCloseCoroutine;
        private List<Button> choiceButtons = new List<Button>();
        private string currentDialogueText;
        private DialogueNode currentNode;
        
        private void Awake()
        {
            // 确保对话UI在开始时隐藏
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            
            if (choicesPanel != null)
            {
                choicesPanel.SetActive(false);
            }
            
            if (continueIcon != null)
            {
                continueIcon.SetActive(false);
            }
            
            // 禁用自身的游戏对象
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
        
        private void OnEnable()
        {
            // 确保DialogueManager存在
            if (DialogueManager.Instance == null)
            {
                Debug.LogError("未找到DialogueManager实例，对话功能可能无法正常工作");
                return;
            }
            
            // 清理选项
            ClearChoices();
            
            // 初始化状态
            isTyping = false;
            isDialogueComplete = false;
            
            // 隐藏继续图标
            if (continueIcon != null)
            {
                continueIcon.SetActive(false);
            }
        }
        
        private void Update()
        {
            // 如果对话面板未激活，不处理任何输入
            if (dialoguePanel == null || !dialoguePanel.activeSelf)
                return;
                
            // 处理继续对话输入
            if (Input.GetKeyDown(continueKey) && !isTyping)
            {
                if (isDialogueComplete)
                {
                    // 如果有选项，等待玩家选择
                    if (choicesPanel != null && choicesPanel.activeSelf)
                    {
                        return;
                    }
                    
                    // 如果没有选项，继续对话
                    if (DialogueManager.Instance != null)
                    {
                        DialogueManager.Instance.ContinueDialogue();
                    }
                }
            }
            // 如果正在打字，按空格跳过打字效果
            else if (Input.GetKeyDown(KeyCode.Space) && isTyping)
            {
                SkipTypewriter();
            }
        }
        
        /// <summary>
        /// 设置对话内容
        /// </summary>
        public void SetDialogue(string text, string speakerName, Sprite speakerImage, SpeakerType speakerType)
        {
            // 确保对话面板显示
            if (dialoguePanel != null && !dialoguePanel.activeSelf)
            {
                dialoguePanel.SetActive(true);
            }
            
            if (speakerNameText != null)
            {
                speakerNameText.text = speakerName;
            }
            
            currentDialogueText = text;
            
            // 设置头像
            if(speakerImage != null)
            {
                this.speakerImage.sprite = speakerImage;
            }
            
            // 显示对话文本
            if (useTypewriterEffect)
            {
                // 停止之前的打字协程
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }
                
                // 开始新的打字协程
                typingCoroutine = StartCoroutine(TypeDialogue(text));
            }
            else
            {
                // 直接显示全部文本
                if (dialogueText != null)
                {
                    dialogueText.text = text;
                }
                
                // 标记对话完成
                isDialogueComplete = true;
                
                // 显示继续图标
                if (continueIcon != null)
                {
                    continueIcon.SetActive(true);
                }
            }
            
            // 如果启用了自动关闭，开始倒计时
            if (autoCloseAfterDelay)
            {
                if (autoCloseCoroutine != null)
                {
                    StopCoroutine(autoCloseCoroutine);
                }
                
                autoCloseCoroutine = StartCoroutine(AutoCloseDialogue());
            }
        }
        
        /// <summary>
        /// 跳过打字机效果
        /// </summary>
        public void SkipTypewriter()
        {
            if (!isTyping) return;
            
            // 停止打字协程
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            
            // 显示全部文本
            if (dialogueText != null)
            {
                dialogueText.text = currentDialogueText;
            }
            
            // 标记状态
            isTyping = false;
            isDialogueComplete = true;
            
            // 显示继续图标
            if (continueIcon != null)
            {
                continueIcon.SetActive(true);
            }
        }
        
        /// <summary>
        /// 设置对话选项
        /// </summary>
        public void SetChoices(List<string> choices)
        {
            // 清理之前的选项
            ClearChoices();
            
            if (choices == null || choices.Count == 0)
            {
                // 如果没有选项，隐藏选项面板
                if (choicesPanel != null)
                {
                    choicesPanel.SetActive(false);
                }
                return;
            }
            
            // 显示选项面板
            if (choicesPanel != null)
            {
                choicesPanel.SetActive(true);
            }
            
            // 创建选项按钮
            for (int i = 0; i < choices.Count; i++)
            {
                GameObject choiceButtonObj = Instantiate(choiceButtonPrefab, choiceButtonsContainer);
                Button choiceButton = choiceButtonObj.GetComponent<Button>();
                TextMeshProUGUI choiceText = choiceButtonObj.GetComponentInChildren<TextMeshProUGUI>();
                
                if (choiceText != null)
                {
                    choiceText.text = choices[i];
                }
                
                int choiceIndex = i; // 创建本地变量以捕获当前索引
                
                if (choiceButton != null)
                {
                    choiceButton.onClick.AddListener(() => OnChoiceSelected(choiceIndex));
                    choiceButtons.Add(choiceButton);
                }
            }
        }
        
        /// <summary>
        /// 清除所有选项
        /// </summary>
        public void ClearChoices()
        {
            // 移除之前创建的所有选项按钮
            foreach (var button in choiceButtons)
            {
                if (button != null)
                {
                    Destroy(button.gameObject);
                }
            }
            
            choiceButtons.Clear();
            
            // 隐藏选项面板
            if (choicesPanel != null)
            {
                choicesPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// 处理选项选择
        /// </summary>
        private void OnChoiceSelected(int choiceIndex)
        {
            if (DialogueManager.Instance != null)
            {
                // 使用反射或间接方式调用，避免编译错误
                if (currentNode != null && currentNode.choices != null && choiceIndex >= 0 && choiceIndex < currentNode.choices.Count)
                {
                    DialogueManager.Instance.ContinueDialogue();
                }
                else
                {
                    Debug.LogWarning($"无效的选项索引: {choiceIndex}");
                }
            }
        }
        
        /// <summary>
        /// 设置说话者风格
        /// </summary>
        private void SetSpeakerStyle(SpeakerType speakerType)
        {
            switch(speakerType)
            {
                case SpeakerType.NPC:
                    dialoguePanel.GetComponent<Image>().color = Color.red;
                    break;
                case SpeakerType.Player:
                    dialoguePanel.GetComponent<Image>().color = Color.blue;
                    break;
                case SpeakerType.Other:
                    dialoguePanel.GetComponent<Image>().color = Color.green;
                    break;
            }
        }
        
        /// <summary>
        /// 打字机效果协程
        /// </summary>
        private IEnumerator TypeDialogue(string text)
        {
            isTyping = true;
            isDialogueComplete = false;
            
            // 隐藏继续图标
            if (continueIcon != null)
            {
                continueIcon.SetActive(false);
            }
            
            if (dialogueText != null)
            {
                dialogueText.text = "";
                
                // 逐字显示文本
                for (int i = 0; i < text.Length; i++)
                {
                    dialogueText.text += text[i];
                    
                    // 播放打字声音
                    if (typingSound != null)
                    {
                        if (text[i] != ' ' && text[i] != '\n')
                        {
                            AudioSource.PlayClipAtPoint(typingSound, Camera.main.transform.position, typingSoundVolume);
                        }
                    }
                    
                    yield return new WaitForSeconds(typingSpeed);
                }
            }
            
            isTyping = false;
            isDialogueComplete = true;
            
            // 显示继续图标
            if (continueIcon != null)
            {
                continueIcon.SetActive(true);
            }
        }
        
        /// <summary>
        /// 自动关闭对话协程
        /// </summary>
        private IEnumerator AutoCloseDialogue()
        {
            // 等待文本完全显示
            while (isTyping)
            {
                yield return null;
            }
            
            // 额外等待自动关闭延迟
            yield return new WaitForSeconds(autoCloseDelay);
            
            // 如果没有选项，自动关闭对话
            if (choicesPanel == null || !choicesPanel.activeSelf)
            {
                // 继续对话或关闭
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ContinueDialogue();
                }
            }
        }
        
        /// <summary>
        /// 关闭对话
        /// </summary>
        public void CloseDialogue()
        {
            // 隐藏对话面板
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            
            // 清理选项
            ClearChoices();
            
            // 停止所有协程
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            
            if (autoCloseCoroutine != null)
            {
                StopCoroutine(autoCloseCoroutine);
                autoCloseCoroutine = null;
            }
            
            // 重置状态
            isTyping = false;
            isDialogueComplete = false;
            
            // 注意：不再调用DialogueManager.EndDialogue()，避免循环引用
            // DialogueManager负责调用这个方法，而不是这个方法调用DialogueManager
        }

        public void SetDialogueNode(DialogueNode node)
        {
            currentNode = node;
        }
    }
}