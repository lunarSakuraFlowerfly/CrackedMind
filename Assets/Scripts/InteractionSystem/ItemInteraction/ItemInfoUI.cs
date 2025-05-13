using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InteractionSystem.Audio;
using InteractionSystem.Data;
using System.Collections;

namespace InteractionSystem.Item
{
    /// <summary>
    /// 物品信息UI，显示物品的详细信息
    /// </summary>
    public class ItemInfoUI : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private GameObject infoPanel;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image itemImage;
        
        [Header("继续图标设置")]
        [SerializeField] private GameObject continueIcon; // 继续图标
        [SerializeField] private float iconMoveRange = 10f; // 图标上下移动范围
        [SerializeField] private float iconMoveSpeed = 1f; // 图标移动速度
        [SerializeField] private bool showContinueIcon = true; // 是否显示继续图标
        
        [Header("UI设置")]
        [SerializeField] private float displayDuration = 0f; // 0表示一直显示直到关闭
        [SerializeField] private bool useTypewriterEffect = true; // 是否使用打字机效果
        [SerializeField] private float typingSpeed = 0.05f; // 打字机效果的速度
        
        [Header("音效设置")]
        [SerializeField] private bool playOpenSound = true; // 是否播放打开音效
        [SerializeField] private bool playCloseSound = true; // 是否播放关闭音效
        [SerializeField] private bool playTypingSound = true; // 是否播放打字机音效
        
        [Header("输入设置")]
        [SerializeField] private KeyCode continueKey = KeyCode.Return; // 继续/关闭的按键
        
        private float displayTimer;
        private string fullDescription;
        private Coroutine typingCoroutine;
        private Coroutine iconAnimationCoroutine; // 图标动画协程
        private bool isTypingComplete = false;
        private Vector3 continueIconOriginalPosition; // 继续图标的原始位置
        
        private void Awake()
        {
            // 确保初始状态为隐藏
            if (infoPanel != null)
            {
                infoPanel.SetActive(false);
            }
            
            // 保存继续图标的原始位置
            if (continueIcon != null)
            {
                continueIconOriginalPosition = continueIcon.transform.localPosition;
                continueIcon.SetActive(false);
            }
        }
        
        private void Update()
        {
            // 如果面板未激活，直接返回
            if (infoPanel == null || !infoPanel.activeSelf) return;
            
            // 如果设置了显示时间，倒计时关闭
            if (displayDuration > 0)
            {
                displayTimer -= Time.deltaTime;
                
                if (displayTimer <= 0)
                {
                    HideItemInfo();
                    return;
                }
            }
            
            // 按ESC键关闭面板
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideItemInfo();
                return;
            }
            
            // 按回车键或鼠标左键继续/关闭
            if (Input.GetKeyDown(continueKey) || Input.GetMouseButtonDown(0))
            {
                HandleContinueOrClose();
            }
            
            // 按空格键跳过打字机效果
            if (Input.GetKeyDown(KeyCode.Space) && useTypewriterEffect && !isTypingComplete)
            {
                SkipTypewriterEffect();
            }
        }
        
        /// <summary>
        /// 处理继续或关闭操作
        /// </summary>
        private void HandleContinueOrClose()
        {
            // 如果正在打字，跳过打字机效果显示全部文本
            if (typingCoroutine != null && !isTypingComplete)
            {
                SkipTypewriterEffect();
            }
            // 如果打字已完成，关闭面板
            else
            {
                HideItemInfo();
            }
        }
        
        /// <summary>
        /// 跳过打字机效果
        /// </summary>
        private void SkipTypewriterEffect()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
                
                if (descriptionText != null)
                {
                    descriptionText.text = fullDescription;
                }
                
                isTypingComplete = true;
                
                // 播放交互完成音效
                if (playCloseSound && InteractionAudioManager.Instance != null)
                {
                    InteractionAudioManager.Instance.PlayInteractionCompleteSound();
                }
            }
        }
        
        /// <summary>
        /// 显示物品信息
        /// </summary>
        public void ShowItemInfo(ItemData itemData)
        {
            if (itemData == null || infoPanel == null) return;
            
            // 播放交互开始音效
            if (playOpenSound && InteractionAudioManager.Instance != null)
            {
                InteractionAudioManager.Instance.PlayInteractionStartSound();
            }
            
            // 设置UI内容
            if (itemNameText != null)
            {
                itemNameText.text = itemData.itemName;
            }
            
            // 保存完整描述
            fullDescription = itemData.description;
            isTypingComplete = false;
            
            // 设置描述文本
            if (descriptionText != null)
            {
                if (useTypewriterEffect)
                {
                    // 使用打字机效果
                    if (typingCoroutine != null)
                    {
                        StopCoroutine(typingCoroutine);
                    }
                    
                    descriptionText.text = "";
                    typingCoroutine = StartCoroutine(TypewriterEffect(fullDescription));
                }
                else
                {
                    // 直接显示全部文本
                    descriptionText.text = fullDescription;
                    isTypingComplete = true;
                }
            }
            
            // 设置物品图片
            if (itemImage != null)
            {
                // 不使用icon属性，因为ItemData中不存在该属性
                itemImage.enabled = false; // 禁用图片显示
                
                // 可以在此设置默认图标或根据物品类型设置不同图标
                // itemImage.sprite = GetDefaultSprite();
                itemImage.preserveAspect = true;
            }
            
            // 显示面板
            infoPanel.SetActive(true);
            
            // 重置计时器
            displayTimer = displayDuration;
        }
        
        /// <summary>
        /// 隐藏物品信息
        /// </summary>
        public void HideItemInfo()
        {
            if (infoPanel != null)
            {
                infoPanel.SetActive(false);
                
                // 播放交互完成音效
                if (playCloseSound && InteractionAudioManager.Instance != null)
                {
                    InteractionAudioManager.Instance.PlayInteractionCompleteSound();
                }
            }
            
            // 停止打字机效果
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            
            // 停止图标动画
            if (iconAnimationCoroutine != null)
            {
                StopCoroutine(iconAnimationCoroutine);
                iconAnimationCoroutine = null;
            }
            
            // 隐藏继续图标
            if (continueIcon != null)
            {
                continueIcon.SetActive(false);
            }
            
            isTypingComplete = false;
        }
        
        /// <summary>
        /// 检查物品信息是否正在显示
        /// </summary>
        public bool IsInfoShowing()
        {
            return infoPanel != null && infoPanel.activeSelf;
        }
        
        /// <summary>
        /// 打字机效果协程
        /// </summary>
        private System.Collections.IEnumerator TypewriterEffect(string text)
        {
            descriptionText.text = "";
            isTypingComplete = false;
            
            foreach (char c in text)
            {
                descriptionText.text += c;
                
                // 播放打字机音效
                if (playTypingSound && InteractionAudioManager.Instance != null)
                {
                    InteractionAudioManager.Instance.PlayTypingSound(c);
                }
                
                // 如果是标点符号，稍微停顿一下
                if (c == '.' || c == ',' || c == '!' || c == '?')
                {
                    yield return new WaitForSeconds(typingSpeed * 4);
                }
                else
                {
                    yield return new WaitForSeconds(typingSpeed);
                }
            }
            
            typingCoroutine = null;
            isTypingComplete = true;
            
            // 打字完成后播放交互完成音效
            if (playCloseSound && InteractionAudioManager.Instance != null)
            {
                InteractionAudioManager.Instance.PlayInteractionCompleteSound();
            }
            
            // 打字完成后显示继续图标并开始动画
            if (showContinueIcon && continueIcon != null)
            {
                continueIcon.SetActive(true);
                if (iconAnimationCoroutine != null)
                {
                    StopCoroutine(iconAnimationCoroutine);
                }
                iconAnimationCoroutine = StartCoroutine(AnimateContinueIcon());
            }
        }
        
        /// <summary>
        /// 继续图标动画协程
        /// </summary>
        private IEnumerator AnimateContinueIcon()
        {
            float time = 0f;
            
            while (true)
            {
                // 使用正弦函数实现上下移动
                float yOffset = Mathf.Sin(time * iconMoveSpeed) * iconMoveRange;
                continueIcon.transform.localPosition = continueIconOriginalPosition + new Vector3(0, yOffset, 0);
                
                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}
