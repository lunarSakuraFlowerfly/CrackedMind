using UnityEngine;
using System.Collections;

public class HitEffectManager : MonoBehaviour
{
    [Header("击中特效设置")]
    [SerializeField] private Vector2 randomPositionMinRange = new Vector2(-0.5f, -1.0f); // 随机位置范围
    [SerializeField] private Vector2 randomPositionMaxRange = new Vector2(0f, -0.3f); // 随机位置范围
    private Vector3 originalLocalPosition; // 原始局部位置
    private Coroutine effectCoroutine; // 效果协程引用
    
    private void Awake()
    {
        // 记录原始局部位置
        originalLocalPosition = transform.localPosition;
        
        // 初始时设置为非激活状态
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 显示击中特效
    /// </summary>
    /// <param name="duration">特效持续时间</param>
    public void ShowHitEffect(float duration,Color color)
    {
        // 如果已经有特效在播放，先停止
        if(effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }
        
        // 激活GameObject
        gameObject.SetActive(true);
        GetComponent<SpriteRenderer>().color = color;
        // 设置随机位置
        SetRandomLocalPosition();
        
        // 开始特效协程
        effectCoroutine = StartCoroutine(HitEffectCoroutine(duration));
        
        Debug.Log($"{gameObject.name} 开始播放击中特效，持续时间: {duration}秒");
    }
    
    /// <summary>
    /// 击中特效协程
    /// </summary>
    private IEnumerator HitEffectCoroutine(float duration)
    {
        // 等待指定时间
        yield return new WaitForSeconds(duration);
        
        // 时间结束后关闭特效
        HideHitEffect();
    }
    
    /// <summary>
    /// 隐藏击中特效
    /// </summary>
    private void HideHitEffect()
    {
        gameObject.SetActive(false);
        effectCoroutine = null;
        
        Debug.Log($"{gameObject.name} 击中特效结束");
    }
    
    /// <summary>
    /// 设置随机局部位置
    /// </summary>
    private void SetRandomLocalPosition()
    {
        // 在原始位置基础上添加随机偏移
        float randomX = Random.Range(randomPositionMinRange.x, randomPositionMaxRange.x);
        float randomY = Random.Range(randomPositionMinRange.y, randomPositionMaxRange.y);
        
        Vector3 randomPosition = originalLocalPosition + new Vector3(randomX, randomY, 0);
        transform.localPosition = randomPosition;
        
        Debug.Log($"HitEffect 随机位置: {randomPosition}");
    }
    
    /// <summary>
    /// 强制停止特效
    /// </summary>
    public void ForceStop()
    {
        if(effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
            effectCoroutine = null;
        }
        
        HideHitEffect();
    }
    

    
    /// <summary>
    /// 重置到原始位置
    /// </summary>
    public void ResetToOriginalPosition()
    {
        transform.localPosition = originalLocalPosition;
    }
    
    private void OnDisable()
    {
        // 当GameObject被禁用时，确保协程被停止
        if(effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
            effectCoroutine = null;
        }
    }
}