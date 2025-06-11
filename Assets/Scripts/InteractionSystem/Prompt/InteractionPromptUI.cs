using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace InteractionSystem.Prompt
{
    /// <summary>
    /// 交互提示UI，显示"按E交互"等提示
    /// </summary>
    public class InteractionPromptUI : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private GameObject promptPanel;
        [SerializeField] private TextMeshProUGUI promptText;
        
        [Header("位置设置")]
        [SerializeField] private Vector2 offset = new Vector2(0, 50);
        [SerializeField] private bool followTarget = true;
        
        private Transform targetTransform;
        private Camera mainCamera;
        
        private void Awake()
        {
            mainCamera = Camera.main;
            
            // 确保初始状态为隐藏
            Hide();
        }
        
        private void LateUpdate()
        {
            // 如果需要跟随目标，更新位置
            if (followTarget && targetTransform != null)
            {
                UpdatePosition();
            }
        }
        
        /// <summary>
        /// 显示交互提示
        /// </summary>
        public void Show(string text, Transform target)
        {
            if (promptText != null)
            {
                promptText.text = text;
            }
            
            targetTransform = target;
            
            if (promptPanel != null)
            {
                promptPanel.SetActive(true);
            }
            
            // 立即更新位置
            if (followTarget && targetTransform != null)
            {
                UpdatePosition();
            }
        }
        
        /// <summary>
        /// 隐藏交互提示
        /// </summary>
        public void Hide()
        {
            if (promptPanel != null)
            {
                promptPanel.SetActive(false);
            }
            
            targetTransform = null;
        }
        
        /// <summary>
        /// 更新提示位置
        /// </summary>
        private void UpdatePosition()
        {
            if (targetTransform == null || mainCamera == null) return;
            
            // 将世界坐标转换为屏幕坐标
            Vector2 screenPos = mainCamera.WorldToScreenPoint(targetTransform.position);
            
            // 应用偏移
            screenPos += offset;
            
            // 设置位置
            if (promptPanel != null)
            {
                promptPanel.transform.position = screenPos;
            }
        }
    }
}
