using System.Collections;
using UnityEngine;

/// <summary>
/// 波纹后处理效果，直接使用提供的材质实现场景捕获和波纹效果
/// </summary>
[RequireComponent(typeof(Camera))]
public class RipplePostProcess : MonoBehaviour
{
    [Header("波纹材质设置")]
    [SerializeField] private Material m_RippleMaterial; // 直接使用此材质，不创建副本

    [Header("场景捕获设置")]
    [SerializeField] private bool m_CaptureSceneOnAwake = true;
    private RenderTexture m_ScreenTexture;
    private Texture2D m_SceneSnapshot;
    
    [Header("调试选项")]
    [SerializeField] private bool m_DebugMode = false;
    
    private bool m_IsEffectEnabled = false;
    private float m_EffectStrength = 0f;
    private Coroutine m_FadeCoroutine;
    
    private Camera m_Camera;
    private int m_LastScreenWidth;
    private int m_LastScreenHeight;
    
    // 添加在类的顶部，作为静态字段
    private static Texture2D s_PersistentSnapshot;
    private static bool s_HasSnapshot = false;
    
    /// <summary>
    /// 初始化组件
    /// </summary>
    private void Awake()
    {
        m_Camera = GetComponent<Camera>();
        if (m_Camera == null)
        {
            Debug.LogError("RipplePostProcess需要Camera组件");
            enabled = false;
            return;
        }
        
        // 初始化渲染纹理
        CreateRenderTexture();
        
        // 检查材质
        if (m_RippleMaterial == null)
        {
            Debug.LogError("未设置波纹材质！该组件将被禁用");
            enabled = false;
            return;
        }
        
        // 确保效果初始为禁用状态
        if (m_RippleMaterial.HasProperty("_RippleStrength"))
        {
            m_RippleMaterial.SetFloat("_RippleStrength", 0);
            m_EffectStrength = 0f;
        }
        else
        {
            Debug.LogError("波纹材质缺少_RippleStrength属性！");
        }
        
        if (m_CaptureSceneOnAwake)
        {
            CaptureSceneSnapshot();
        }
        
        // 保存当前分辨率
        m_LastScreenWidth = Screen.width;
        m_LastScreenHeight = Screen.height;
    }
    
    /// <summary>
    /// 检查屏幕分辨率变化
    /// </summary>
    private void Update()
    {
        // 检测分辨率变化
        if (Screen.width != m_LastScreenWidth || Screen.height != m_LastScreenHeight)
        {
            if (m_DebugMode)
            {
                Debug.Log($"<color=yellow>屏幕分辨率已变化: {m_LastScreenWidth}x{m_LastScreenHeight} -> {Screen.width}x{Screen.height}</color>");
            }
            
            // 重新创建渲染纹理
            CreateRenderTexture();
            
            // 如果效果已启用，重新捕获场景
            if (m_IsEffectEnabled)
            {
                CaptureSceneSnapshot();
            }
            
            // 更新保存的分辨率
            m_LastScreenWidth = Screen.width;
            m_LastScreenHeight = Screen.height;
        }
    }
    
    /// <summary>
    /// 创建用于场景捕获的渲染纹理
    /// </summary>
    private void CreateRenderTexture()
    {
        if (m_ScreenTexture != null)
        {
            m_ScreenTexture.Release();
            Destroy(m_ScreenTexture);
        }
        
        m_ScreenTexture = new RenderTexture(Screen.width, Screen.height, 24);
        m_ScreenTexture.name = "RippleScreenTexture";
        m_ScreenTexture.filterMode = FilterMode.Bilinear;
        m_ScreenTexture.wrapMode = TextureWrapMode.Clamp;
        
        if (m_SceneSnapshot != null)
        {
            Destroy(m_SceneSnapshot);
        }
        
        m_SceneSnapshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        m_SceneSnapshot.name = "SceneSnapshot";
        m_SceneSnapshot.filterMode = FilterMode.Bilinear;
        m_SceneSnapshot.wrapMode = TextureWrapMode.Clamp;
        
        if (m_DebugMode)
        {
            Debug.Log($"<color=cyan>已创建渲染纹理，分辨率: {Screen.width}x{Screen.height}</color>");
        }
    }
    
    /// <summary>
    /// 捕获当前屏幕内容作为场景快照
    /// </summary>
    public void CaptureSceneSnapshot(bool makePersistent = false)
    {
        if (m_ScreenTexture == null || m_SceneSnapshot == null)
        {
            CreateRenderTexture();
        }
        
        try
        {
            // 先进行一次渲染，确保获得最新画面
            Camera cam = m_Camera != null ? m_Camera : Camera.main;
            if (cam != null)
            {
                // 保存原始设置
                RenderTexture originalRT = cam.targetTexture;
                
                try
                {
                    // 将相机渲染到我们的纹理
                    cam.targetTexture = m_ScreenTexture;
                    cam.Render();
                    cam.targetTexture = originalRT;
                    
                    // 读取像素数据
                    RenderTexture.active = m_ScreenTexture;
                    m_SceneSnapshot.ReadPixels(new Rect(0, 0, m_ScreenTexture.width, m_ScreenTexture.height), 0, 0);
                    m_SceneSnapshot.Apply();
                    RenderTexture.active = null;
                    
                    // 将快照应用到材质
                    if (m_RippleMaterial != null && m_RippleMaterial.HasProperty("_MainTex"))
                    {
                        m_RippleMaterial.SetTexture("_MainTex", m_SceneSnapshot);
                        if (m_DebugMode)
                        {
                            Debug.Log("<color=green>场景快照已更新到材质</color>");
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"捕获场景快照时出错: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("无法找到相机捕获场景");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"场景捕获过程中出错: {e.Message}");
        }
        
        // 在成功捕获后保存一份持久化的快照
        if (makePersistent && m_SceneSnapshot != null)
        {
            if (s_PersistentSnapshot != null) Destroy(s_PersistentSnapshot);
            
            // 创建新的持久快照并复制当前快照的内容
            s_PersistentSnapshot = new Texture2D(m_SceneSnapshot.width, m_SceneSnapshot.height, m_SceneSnapshot.format, false);
            s_PersistentSnapshot.SetPixels(m_SceneSnapshot.GetPixels());
            s_PersistentSnapshot.Apply();
            
            s_HasSnapshot = true;
            if (m_DebugMode) Debug.Log("<color=cyan>已创建持久化场景快照</color>");
        }
    }
    
    /// <summary>
    /// 启用波纹效果，将波纹强度从0渐变到目标值
    /// </summary>
    /// <param name="duration">过渡持续时间（秒）</param>
    /// <param name="strength">目标波纹强度(0-1)</param>
    public void EnableEffect(float duration = 0.5f, float strength = 0.5f)
    {
        EnableAdvancedEffect(duration, strength, 0.2f, 0.0f);
    }
    
    /// <summary>
    /// 禁用波纹效果，将波纹强度从当前值渐变到0
    /// </summary>
    /// <param name="duration">过渡持续时间（秒）</param>
    public void DisableEffect(float duration = 0.5f)
    {
        EnableAdvancedEffect(duration, 0.0f, 0.2f, 0.0f, () => {
            m_IsEffectEnabled = false;
        });   
    }
    
    /// <summary>
    /// 获取当前场景快照
    /// </summary>
    /// <returns>场景快照纹理</returns>
    public Texture2D GetSceneSnapshot()
    {
        return m_SceneSnapshot;
    }

    /// <summary>
    /// 渲染图像，应用波纹效果
    /// </summary>
    /// <param name="source">源渲染纹理</param>
    /// <param name="destination">目标渲染纹理</param>
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // 如果效果禁用或材质不存在或Canvas禁用，直接传递图像
        if (!m_IsEffectEnabled || m_RippleMaterial == null || m_EffectStrength <= 0.001f)
        {
            Graphics.Blit(source, destination);
            return;
        }
        
        // 应用波纹效果
        Graphics.Blit(source, destination, m_RippleMaterial);
    }
    
    /// <summary>
    /// 组件禁用时释放资源
    /// </summary>
    private void OnDisable()
    {
        ReleaseResources();
    }
    
    /// <summary>
    /// 组件销毁时释放资源
    /// </summary>
    private void OnDestroy()
    {
        ReleaseResources();
    }
    
    /// <summary>
    /// 释放所有资源
    /// </summary>
    private void ReleaseResources()
    {
        // 释放资源
        if (m_ScreenTexture != null)
        {
            m_ScreenTexture.Release();
            Destroy(m_ScreenTexture);
            m_ScreenTexture = null;
        }
        
        if (m_SceneSnapshot != null)
        {
            Destroy(m_SceneSnapshot);
            m_SceneSnapshot = null;
        }
        
        // 停止所有协程
        if (m_FadeCoroutine != null)
        {
            StopCoroutine(m_FadeCoroutine);
            m_FadeCoroutine = null;
        }
    }

    /// <summary>
    /// 高级波纹效果，同时控制多个参数的渐变
    /// </summary>
    /// <param name="duration">过渡持续时间（秒）</param>
    /// <param name="targetStrength">目标波纹强度(0-1)</param>
    /// <param name="targetSpeed">目标波纹速度</param>
    /// <param name="targetDarkness">目标黑暗度(0-1)</param>
    /// <param name="onComplete">效果完成后的回调</param>
    public void EnableAdvancedEffect(float duration, float targetStrength, float targetSpeed, float targetDarkness, System.Action onComplete = null)
    {
        // 先捕获当前场景
        CaptureSceneSnapshot();
        
        // 启用多参数渐变
        if (m_FadeCoroutine != null)
            StopCoroutine(m_FadeCoroutine);
        
        m_FadeCoroutine = StartCoroutine(FadeMultipleParameters(duration, targetStrength, targetSpeed, targetDarkness, onComplete));
        m_IsEffectEnabled = targetStrength > 0.001f;
    }

    /// <summary>
    /// 平滑过渡波纹参数
    /// </summary>
    /// <param name="duration">过渡持续时间（秒）</param>
    /// <param name="targetStrength">目标波纹强度(0-1)</param>
    /// <param name="targetSpeed">目标波纹速度</param>
    /// <param name="targetDarkness">目标黑暗度(0-1)</param>
    /// <param name="onComplete">效果完成后的回调</param>
    /// <returns>协程枚举器</returns>
    private IEnumerator FadeMultipleParameters(float duration, float targetStrength, float targetSpeed, float targetDarkness, System.Action onComplete = null)
    {
        // 获取初始值
        float startStrength = m_RippleMaterial.HasProperty("_RippleStrength") ? 
                              m_RippleMaterial.GetFloat("_RippleStrength") : 0f;
        float startSpeed = m_RippleMaterial.HasProperty("_RippleSpeed") ? 
                           m_RippleMaterial.GetFloat("_RippleSpeed") : 0.2f;
        float startDarkness = m_RippleMaterial.HasProperty("_DarknessAmount") ? 
                              m_RippleMaterial.GetFloat("_DarknessAmount") : 0f;
        
        float time = 0;
        while (time < duration)
        {
            float t = time / duration;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            
            // 设置波纹强度
            if (m_RippleMaterial.HasProperty("_RippleStrength"))
            {
                float currentStrength = Mathf.Lerp(startStrength, targetStrength, smoothT);
                m_RippleMaterial.SetFloat("_RippleStrength", currentStrength);
                m_EffectStrength = currentStrength;
            }
            
            // 设置波纹速度
            if (m_RippleMaterial.HasProperty("_RippleSpeed"))
                m_RippleMaterial.SetFloat("_RippleSpeed", Mathf.Lerp(startSpeed, targetSpeed, smoothT));
            
            // 设置黑暗度
            if (m_RippleMaterial.HasProperty("_DarknessAmount"))
                m_RippleMaterial.SetFloat("_DarknessAmount", Mathf.Lerp(startDarkness, targetDarkness, smoothT));
            
            time += Time.deltaTime;
            yield return null;
        }
        
        // 设置最终值
        if (m_RippleMaterial.HasProperty("_RippleStrength"))
        {
            m_RippleMaterial.SetFloat("_RippleStrength", targetStrength);
            m_EffectStrength = targetStrength;
        }
        
        if (m_RippleMaterial.HasProperty("_RippleSpeed"))
            m_RippleMaterial.SetFloat("_RippleSpeed", targetSpeed);
        
        if (m_RippleMaterial.HasProperty("_DarknessAmount"))
            m_RippleMaterial.SetFloat("_DarknessAmount", targetDarkness);
        
        // 调用完成回调
        onComplete?.Invoke();
    }

    // 添加一个使用持久化快照的方法
    public bool UsePersistentSnapshot()
    {
        if (!s_HasSnapshot || s_PersistentSnapshot == null)
        {
            if (m_DebugMode) Debug.LogWarning("<color=yellow>没有可用的持久化快照</color>");
            return false;
        }
        
        // 将持久化快照应用到材质
        if (m_RippleMaterial != null && m_RippleMaterial.HasProperty("_MainTex"))
        {
            m_RippleMaterial.SetTexture("_MainTex", s_PersistentSnapshot);
            if (m_DebugMode) Debug.Log("<color=green>已应用持久化快照到材质</color>");
            return true;
        }
        
        return false;
    }

    // 添加设置材质的方法
    public void SetRippleMaterial(Material material)
    {
        if (material != null)
        {
            m_RippleMaterial = material;
            if (m_DebugMode)
            {
                Debug.Log("<color=green>已设置新的波纹材质</color>");
            }
        }
    }
    
    // 添加获取材质的方法
    public Material GetRippleMaterial()
    {
        return m_RippleMaterial;
    }
}


