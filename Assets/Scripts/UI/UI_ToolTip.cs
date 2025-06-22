using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.ShaderGraph.Internal;

public class UI_ToolTip : MonoBehaviour
{
    [Header("边界限制（屏幕比例 0-1）")]
    [SerializeField] [Range(0f, 1f)] private float xLimitRatio = 0.6f; // 屏幕宽度的60%
    [SerializeField] [Range(0f, 1f)] private float yLimitRatio = 0.6f; // 屏幕高度的60%

    [Header("偏移量（屏幕比例 0-1）")]
    [SerializeField] [Range(0f, 0.3f)] private float xOffsetRatio = 0.15f; // 屏幕宽度的15%
    [SerializeField] [Range(0f, 0.3f)] private float yOffsetRatio = 0.15f; // 屏幕高度的15%

    [Header("边距（屏幕比例 0-1）")]
    [SerializeField] [Range(0f, 0.1f)] private float screenMarginRatio = 0.02f; // 屏幕边缘2%的边距

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void AdjustPosition()
    {
        // 确保 rectTransform 已初始化
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Debug.LogError("UI_ToolTip: 找不到RectTransform组件！");
                return;
            }
        }

        Vector2 mousePosition = Input.mousePosition;
        
        // 计算动态的限制值和偏移量
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        float xLimit = screenWidth * xLimitRatio;
        float yLimit = screenHeight * yLimitRatio;
        
        float xOffset = screenWidth * xOffsetRatio;
        float yOffset = screenHeight * yOffsetRatio;
        
        float screenMargin = Mathf.Min(screenWidth, screenHeight) * screenMarginRatio;

        // 计算偏移方向
        float newXOffset = mousePosition.x > xLimit ? -xOffset : xOffset;
        float newYOffset = mousePosition.y > yLimit ? -yOffset : yOffset;

        // 计算初始位置
        Vector2 targetPosition = new Vector2(
            mousePosition.x + newXOffset, 
            mousePosition.y + newYOffset
        );

        // 获取UI元素的尺寸
        Vector2 tooltipSize = rectTransform.sizeDelta;
        
        // 确保Tooltip不超出屏幕边界
        targetPosition = ClampToScreen(targetPosition, tooltipSize, screenMargin);

        transform.position = targetPosition;
    }

    /// <summary>
    /// 将Tooltip位置限制在屏幕范围内
    /// </summary>
    /// <param name="position">目标位置</param>
    /// <param name="tooltipSize">Tooltip尺寸</param>
    /// <param name="margin">屏幕边距</param>
    /// <returns>修正后的位置</returns>
    private Vector2 ClampToScreen(Vector2 position, Vector2 tooltipSize, float margin)
    {
        // 如果尺寸为0，使用默认值避免错误
        if (tooltipSize.x <= 0) tooltipSize.x = 100f;
        if (tooltipSize.y <= 0) tooltipSize.y = 50f;

        float halfWidth = tooltipSize.x * 0.5f;
        float halfHeight = tooltipSize.y * 0.5f;

        // 限制X轴位置
        position.x = Mathf.Clamp(position.x, 
            margin + halfWidth, 
            Screen.width - margin - halfWidth);

        // 限制Y轴位置
        position.y = Mathf.Clamp(position.y, 
            margin + halfHeight, 
            Screen.height - margin - halfHeight);

        return position;
    }

    /// <summary>
    /// 设置相对屏幕的偏移比例
    /// </summary>
    /// <param name="xRatio">X轴偏移比例</param>
    /// <param name="yRatio">Y轴偏移比例</param>
    public void SetOffsetRatio(float xRatio, float yRatio)
    {
        xOffsetRatio = Mathf.Clamp01(xRatio);
        yOffsetRatio = Mathf.Clamp01(yRatio);
    }

    /// <summary>
    /// 设置边界检测比例
    /// </summary>
    /// <param name="xRatio">X轴边界比例</param>
    /// <param name="yRatio">Y轴边界比例</param>
    public void SetLimitRatio(float xRatio, float yRatio)
    {
        xLimitRatio = Mathf.Clamp01(xRatio);
        yLimitRatio = Mathf.Clamp01(yRatio);
    }


}