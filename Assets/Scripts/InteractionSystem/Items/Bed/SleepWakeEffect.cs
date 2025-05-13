using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Reflection;

public class SleepWakeEffect : MonoBehaviour
{
    [Header("基本设置")]
    [SerializeField] private Image m_BlackoutImage;
    [SerializeField] private float m_FadeDuration = 2f;
    [SerializeField] private Material m_RippleMaterial;
    [SerializeField] private float m_RippleDuration = 1.5f;
    [SerializeField] private float m_RippleStrength = 0.5f;
    
    // 材质属性名称常量
    private const string ColorPropertyName = "_Color";
    
    [Header("场景设置")]
    [SerializeField] private string m_DreamSceneName;
    [SerializeField] private string m_RealWorldSceneName;
    [SerializeField] private Transform m_BedPosition;
    
    [Header("玩家引用")]
    [SerializeField] private GameObject m_PlayerObject;
    
    [Header("调试选项")]
    [SerializeField] private bool m_DebugMode = true;
    [SerializeField] private bool m_AutoCreateUI = true;  // 自动创建UI
    
    // 状态跟踪
    private bool m_IsInDream = false;
    private Vector3 m_SleepPosition;
    private RipplePostProcess m_RipplePostProcess;
    private Canvas m_Canvas;
    
    // 修改字段声明部分，添加静态引用以避免重复创建
    private static Canvas s_SharedCanvas = null;
    
    // 添加一个标记来追踪场景切换状态
    private bool m_IsTransitioning = false;
    
    // 添加一个静态实例以在场景间保持引用
    private static SleepWakeEffect s_Instance;

    // 添加公共访问器
    public static SleepWakeEffect Instance => s_Instance;
    
    // 添加场景切换事件
    public System.Action OnBeforeFadeOut;
    public System.Action OnAfterFadeOut;
    public System.Action OnBeforeFadeIn;
    public System.Action OnAfterFadeIn;
    
    private void Awake()
    {
        // 先检查是否已经有实例
        if (s_Instance != null && s_Instance != this)
        {

            Destroy(gameObject);
            return;
        }

        // 在设置实例之前就调用 DontDestroyOnLoad
        DontDestroyOnLoad(gameObject);
        s_Instance = this;
        
        // 首先检查是否已经有其他SleepWakeEffect创建了Canvas
        if (s_SharedCanvas != null)
        {
            m_Canvas = s_SharedCanvas;
            if (m_DebugMode) Debug.Log("<color=cyan>使用已存在的共享SleepCanvas</color>");
        }
        else
        {
            // 尝试查找场景中已存在的SleepCanvas
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();
            foreach (Canvas canvas in allCanvases)
            {
                if (canvas.name == "SleepCanvas")
                {
                    m_Canvas = canvas;
                    s_SharedCanvas = canvas;
                    if (m_DebugMode) Debug.Log("<color=cyan>找到现有SleepCanvas</color>");
                    break;
                }
            }
        }
        
        // 确保游戏一开始就禁用Canvas
        if (m_Canvas != null)
        {
            m_Canvas.gameObject.SetActive(false);
        }
    }
    
    // 修改Start方法，确保Canvas创建逻辑不重复
    private void Start()
    {
        if (m_DebugMode) Debug.Log("<color=blue>SleepWakeEffect.Start()</color>");
        
        // 先查找RipplePostProcess组件，可能需要从中获取材质
        SetupRipplePostProcess();
        
        // 只有在确实需要且Canvas不存在时才创建UI
        if (m_AutoCreateUI && m_BlackoutImage == null && m_Canvas == null)
        {
            CreateBlackoutUI();
        }
        else if (m_Canvas != null && m_BlackoutImage == null)
        {
            // 如果已有Canvas但没有BlackoutImage，尝试查找或创建BlackoutImage
            FindOrCreateBlackoutImage();
        }
        else if (m_BlackoutImage != null)
        {
            // 如果已有黑屏图像，确保材质正确设置
            SetMaterialToBlackoutImage();
            
            m_BlackoutImage.color = new Color(1, 1, 1, 1);
        }
        
        // 确保Canvas一开始是隐藏的
        if (m_Canvas != null)
        {
            m_Canvas.gameObject.SetActive(false);
        }
        
        // 自动查找玩家
        if (m_PlayerObject == null)
        {
            m_PlayerObject = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //调用醒来
        WakeUp();    
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    // 新增一个方法来查找或创建BlackoutImage
    private void FindOrCreateBlackoutImage()
    {
        if (m_Canvas == null) return;
        
        // 先查找现有的BlackoutPanel
        Transform panelTransform = m_Canvas.transform.Find("BlackoutPanel");
        GameObject panelObj;
        
        if (panelTransform != null)
        {
            panelObj = panelTransform.gameObject;
            
            // 查找现有的BlackoutImage
            Transform imageTransform = panelTransform.Find("BlackoutImage");
            if (imageTransform != null)
            {
                m_BlackoutImage = imageTransform.GetComponent<Image>();
                if (m_BlackoutImage != null)
                {
                    // 设置材质
                    SetMaterialToBlackoutImage();
                    return;
                }
            }
        }
        else
        {
            // 创建Panel
            panelObj = new GameObject("BlackoutPanel");
            panelObj.transform.SetParent(m_Canvas.transform, false);
            
            // 添加Panel组件
            panelObj.AddComponent<CanvasRenderer>();
            Image panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0); // 完全透明
            panelImage.raycastTarget = false;
            
            // 设置Panel覆盖整个屏幕
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
            panelRect.anchoredPosition = Vector2.zero;
        }
        
        // 创建BlackoutImage
        GameObject blackoutObj = new GameObject("BlackoutImage");
        blackoutObj.transform.SetParent(panelObj.transform, false);
        m_BlackoutImage = blackoutObj.AddComponent<Image>();
        
        // 设置图像覆盖整个屏幕
        m_BlackoutImage.rectTransform.anchorMin = Vector2.zero;
        m_BlackoutImage.rectTransform.anchorMax = Vector2.one;
        m_BlackoutImage.rectTransform.sizeDelta = Vector2.zero;
        m_BlackoutImage.rectTransform.anchoredPosition = Vector2.zero;
        
        m_BlackoutImage.color = new Color(1, 1, 1, 1);
        
        // 关闭Image的RaycastTarget属性，以免阻挡点击
        m_BlackoutImage.raycastTarget = false;
        
        // 设置材质 - 注意先检查m_RippleMaterial是否存在
        SetMaterialToBlackoutImage();
    }
    
    // 添加一个专用方法设置材质，方便调试和确保材质设置正确
    private void SetMaterialToBlackoutImage()
    {
        if (m_BlackoutImage == null)
        {
            Debug.LogError("<color=red>无法设置材质：BlackoutImage为null</color>");
            return;
        }
        
        // 先确认材质存在
        if (m_RippleMaterial == null)
        {
            Debug.LogWarning("<color=yellow>波纹材质为空，尝试查找可用材质</color>");
            FindRippleMaterial();
        }
        
        // 再次检查确保材质找到了
        if (m_RippleMaterial != null)
        {
            m_BlackoutImage.material = m_RippleMaterial;
            
            // 确保效果初始禁用
            if (m_RippleMaterial.HasProperty("_RippleStrength"))
            {
                m_RippleMaterial.SetFloat("_RippleStrength", 0f);
            }
            
            if (m_DebugMode)
            {
                Debug.Log($"<color=green>成功设置波纹材质: {m_RippleMaterial.name} 到 BlackoutImage</color>");
            }
        }
        else
        {
            if (m_DebugMode)
            {
                Debug.LogError("<color=red>无法设置材质：未找到适合的波纹材质</color>");
            }
        }
    }
    
    // 添加一个方法来查找波纹材质
    private void FindRippleMaterial()
    {
        // 先尝试查找带有"Ripple"名称的材质
        Material[] allMaterials = Resources.FindObjectsOfTypeAll<Material>();
        foreach (Material mat in allMaterials)
        {
            if (mat.name.Contains("Ripple"))
            {
                m_RippleMaterial = mat;
                if (m_DebugMode) Debug.Log($"<color=green>找到波纹材质: {mat.name}</color>");
                return;
            }
        }
        
        // 再尝试在已有RipplePostProcess组件中寻找
        if (m_RipplePostProcess != null)
        {
            // 获取RipplePostProcess的材质
            Material rippleMat = null;
            
            // 尝试通过反射获取私有字段
            System.Type type = m_RipplePostProcess.GetType();
            System.Reflection.FieldInfo field = type.GetField("m_RippleMaterial", 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Public);
            
            if (field != null)
            {
                rippleMat = field.GetValue(m_RipplePostProcess) as Material;
                if (rippleMat != null)
                {
                    m_RippleMaterial = rippleMat;
                    if (m_DebugMode) Debug.Log($"<color=green>从RipplePostProcess获取到波纹材质</color>");
                }
            }
        }
    }
    
    // 修改CreateBlackoutUI方法，优先查找现有Canvas
    private void CreateBlackoutUI()
    {
        // 首先检查是否已经有静态引用的Canvas
        if (s_SharedCanvas != null)
        {
            m_Canvas = s_SharedCanvas;
            
            // 如果已有Canvas，只需要处理BlackoutImage
            FindOrCreateBlackoutImage();
            return;
        }
        
        // 查找场景中已存在的SleepCanvas
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        bool foundSleepCanvas = false;
        
        // 先特别寻找名为SleepCanvas的对象
        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.name == "SleepCanvas")
            {
                m_Canvas = canvas;
                s_SharedCanvas = canvas;
                foundSleepCanvas = true;
                break;
            }
        }
        
        // 如果没有找到专门的SleepCanvas，但找到了其他可用的Canvas
        if (!foundSleepCanvas)
        {
            // 再尝试查找包含BlackoutPanel的Canvas
            foreach (Canvas canvas in allCanvases)
            {
                Transform panelTransform = canvas.transform.Find("BlackoutPanel");
                if (panelTransform != null)
                {
                    m_Canvas = canvas;
                    break;
                }
            }
        }
        
        // 如果仍然没有找到Canvas，创建一个
        if (m_Canvas == null)
        {
            GameObject canvasObj = new GameObject("SleepCanvas");
            m_Canvas = canvasObj.AddComponent<Canvas>();
            m_Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            m_Canvas.sortingOrder = 100; // 确保在最上层显示
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 保存静态引用
            s_SharedCanvas = m_Canvas;
        }
        
        // 处理BlackoutImage
        FindOrCreateBlackoutImage();
        
        // 确保创建后Canvas立即禁用
        if (m_Canvas != null)
        {
            m_Canvas.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 设置波纹后处理效果
    /// </summary>
    private void SetupRipplePostProcess()
    {
        // 查找主摄像机
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("<color=red>未找到主摄像机！无法设置波纹后处理效果。</color>");
            return;
        }
        
        // 查找或添加RipplePostProcess组件
        m_RipplePostProcess = mainCamera.GetComponent<RipplePostProcess>();
        if (m_RipplePostProcess == null)
        {
            m_RipplePostProcess = mainCamera.gameObject.AddComponent<RipplePostProcess>();
        }
        
        // 确保波纹材质已设置
        if (m_RippleMaterial == null)
        {
            FindRippleMaterial();
        }
        
        // 如果有BlackoutImage但还没设置材质，现在设置
        if (m_BlackoutImage != null && (m_BlackoutImage.material == null || m_BlackoutImage.material != m_RippleMaterial))
        {
            SetMaterialToBlackoutImage();
        }
    }
    
    /// <summary>
    /// 开始睡眠并进入梦境
    /// </summary>
    public void StartSleep()
    {
        
        // 激活Canvas
        if (m_Canvas != null)
        {
            m_Canvas.gameObject.SetActive(true);
        }
        
        // 检查必要组件
        if (m_BlackoutImage == null)
        {
            Debug.LogError("<color=red>错误：未设置BlackoutImage</color>");
        }
        
        if (string.IsNullOrEmpty(m_DreamSceneName))
        {
            Debug.LogError("<color=red>错误：未设置梦境场景名称</color>");
        }
        
        // 保存睡眠位置
        if (m_PlayerObject != null)
        {
            m_SleepPosition = m_PlayerObject.transform.position;
        }
        
        // 开始淡入效果
        StartCoroutine(SleepSequence());
    }
    
    /// <summary>
    /// 从梦境醒来
    /// </summary>
    public void WakeUp()
    {
        if (m_IsTransitioning)
        {
            if (m_DebugMode) Debug.LogWarning("<color=orange>正在过渡中，无法再次调用</color>");
            return;
        }
        //重新设置玩家引用 通过名字和tag查询
        if (m_PlayerObject == null)
        {
            m_PlayerObject = GameObject.FindGameObjectWithTag("Player");
        }
        
        StartCoroutine(WakeUpSequence());
    }
    
    /// <summary>
    /// 睡眠序列：专注于淡入效果（屏幕逐渐变暗并加载场景）
    /// </summary>
    private IEnumerator SleepSequence()
    {
        m_IsTransitioning = true;
        
        // 调用淡入前事件
        OnBeforeFadeOut?.Invoke();
        
        // 禁用玩家控制
        DisablePlayerControl();
        
        // 确保RipplePostProcess组件存在
        if (m_RipplePostProcess == null)
        {
            SetupRipplePostProcess();
        }
        
        // 捕获当前场景
        if (m_RipplePostProcess != null)
        {
            m_RipplePostProcess.CaptureSceneSnapshot();
        }
        
        // 等待一小段时间确保快照已更新
        yield return new WaitForSeconds(0.1f);
        
        // 激活Canvas
        if (m_Canvas != null && !m_Canvas.gameObject.activeSelf)
        {
            m_Canvas.gameObject.SetActive(true);
        }
        
        // 创建一个等待完成的信号
        bool effectCompleted = false;
        System.Action onEffectComplete = () => { effectCompleted = true; };
        
        if (m_RipplePostProcess != null)
        {
            // 启用波纹效果，并设置黑暗度和速度
            m_RipplePostProcess.EnableAdvancedEffect(
                m_RippleDuration,    // 持续时间
                m_RippleStrength,    // 目标强度
                1.0f,                // 目标速度
                1.0f,                // 目标黑暗度
                onEffectComplete     // 完成回调
            );
            
            // 等待波纹效果完成或最长等待时间
            float waitTime = 0;
            while (!effectCompleted && waitTime < m_RippleDuration + 0.5f)
            {
                waitTime += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            // 如果没有波纹效果，使用普通淡入黑屏
            yield return StartCoroutine(FadeToBlack());
        }
        
        // 淡入效果完成，调用淡入后事件
        OnAfterFadeOut?.Invoke();
        
        // 加载梦境场景
        if (!string.IsNullOrEmpty(m_DreamSceneName))
        {
            LoadDreamScene();
        }
        
        m_IsTransitioning = false;
        //添加场景切换完成后事件
    }
    
    /// <summary>
    /// 唤醒序列：专注于淡出效果（屏幕逐渐变亮）
    /// </summary>
    private IEnumerator WakeUpSequence()
    {
        // 防止重复调用
        if (m_IsTransitioning)
        {
            if (m_DebugMode) Debug.LogWarning("<color=yellow>正在执行过渡效果，请等待当前过渡完成</color>");
            yield break;
        }

        #region 初始化
        //在场景中寻找名为WakeCanvas的Canvas
        Canvas[] allCanvases = Resources.FindObjectsOfTypeAll<Canvas>();
        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.name == "WakeCanvas")
            {
                if (m_DebugMode) Debug.Log($"<color=green>找到Canvas: {canvas.name}，激活状态: {canvas.gameObject.activeSelf}</color>");
                m_Canvas = canvas;
            }
        }

        m_Canvas.gameObject.SetActive(true);
        //在子对象的panel的子对象中寻找image
        m_BlackoutImage = m_Canvas.transform.Find("WakePanel").Find("WakeImage").GetComponent<Image>();
        m_IsTransitioning = true;
        OnBeforeFadeIn?.Invoke();
        DisablePlayerControl();
        #endregion

        #region 设置后处理效果
        // 确保RipplePostProcess组件存在
        if (m_RipplePostProcess == null)
        {
            SetupRipplePostProcess();
        }

        // 设置材质初始参数
        if (m_RippleMaterial != null)
        {
            m_RippleMaterial.SetFloat("_RippleStrength", m_RippleStrength);
            if (m_RippleMaterial.HasProperty("_DarknessFactor"))
            {
                m_RippleMaterial.SetFloat("_DarknessFactor", 1.0f);
            }
        }
        #endregion

        #region 场景捕获
        // 捕获当前场景
        if (m_RipplePostProcess != null)
        {
            m_RipplePostProcess.CaptureSceneSnapshot();
            yield return new WaitForSeconds(0.1f); // 等待快照更新
        }

        // 激活Canvas
        if (m_Canvas != null && !m_Canvas.gameObject.activeSelf)
        {
            m_Canvas.gameObject.SetActive(true);
        }
        #endregion

        #region 执行过渡效果
        if (m_RipplePostProcess != null)
        {
            // 设置过渡参数
            bool effectCompleted = false;
            System.Action onEffectComplete = () => { effectCompleted = true; };
            float transitionDuration = m_RippleDuration * 1.5f;

            // 启动波纹过渡
            m_RipplePostProcess.EnableAdvancedEffect(
                transitionDuration,    // 延长持续时间使过渡更平滑
                0f,                    // 目标波纹强度
                0.0f,                  // 目标速度
                0.0f,                  // 目标黑暗度
                onEffectComplete       // 完成回调
            );

            // 等待效果完成
            float waitTime = 0;
            float maxWaitTime = transitionDuration + 0.5f;
            
            while (!effectCompleted && waitTime < maxWaitTime)
            {
                if (m_DebugMode)
                {
                    float progress = (waitTime / transitionDuration) * 100;
                    Debug.Log($"<color=cyan>过渡进度: {progress:F1}%</color>");
                }
                waitTime += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            // 如果没有波纹效果，使用普通淡出效果
            yield return StartCoroutine(FadeToClear());
        }
        #endregion

        #region 完成清理
        OnAfterFadeIn?.Invoke();
        
        if (m_Canvas != null)
        {
            m_Canvas.gameObject.SetActive(false);
        }

        EnablePlayerControl();
        m_IsTransitioning = false;
        #endregion
    }
    
    // 加载梦境场景
    private void LoadDreamScene()
    {
        if (string.IsNullOrEmpty(m_DreamSceneName))
        {
            Debug.LogWarning("<color=orange>未设置梦境场景，将进入黑屏睡眠状态</color>");
            return;
        }
        
        // 检查场景是否存在
        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == m_DreamSceneName)
            {
                sceneExists = true;
                break;
            }
        }
        
        if (!sceneExists)
        {
            Debug.LogError($"<color=red>梦境场景 '{m_DreamSceneName}' 不在Build Settings中!</color>");
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        // 加载场景
        SceneManager.LoadScene(m_DreamSceneName);
        
        // 设置梦境状态
        m_IsInDream = true;
        //添加场景切换完成后事件
        
    }
    
    /// <summary>
    /// 屏幕淡入黑色协程
    /// </summary>
    private IEnumerator FadeToBlack()
    {
        if (m_BlackoutImage == null)
        {
            Debug.LogError("<color=red>黑屏图像为空!</color>");
            yield break;
        }
        
        // 显示图像，设置为不透明白色
        m_BlackoutImage.color = Color.white;
        
        // 动态设置RippleStrength属性，从0渐变到设定值
        if (m_BlackoutImage.material != null && m_BlackoutImage.material.HasProperty("_RippleStrength"))
        {
            m_BlackoutImage.material.SetFloat("_RippleStrength", 0f);
            
            float timeElapsed = 0f;
            while (timeElapsed < m_FadeDuration)
            {
                // 从0到设定的波纹强度，逐渐开启效果
                float effectValue = Mathf.Lerp(0, m_RippleStrength, timeElapsed / m_FadeDuration);
                m_BlackoutImage.material.SetFloat("_RippleStrength", effectValue);
                
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            
            // 设置最终值
            m_BlackoutImage.material.SetFloat("_RippleStrength", m_RippleStrength);
        }
        else
        {
            // 等待相同的时间，以保持流程一致
            yield return new WaitForSeconds(m_FadeDuration);
        }
    }
    
    /// <summary>
    /// 屏幕淡出至透明协程
    /// </summary>
    private IEnumerator FadeToClear()
    {
        if (m_BlackoutImage == null) yield break;
        
        // 确保UI图像颜色为白色
        m_BlackoutImage.color = Color.white;
        
        // 从设定值淡出到0
        if (m_BlackoutImage.material != null && m_BlackoutImage.material.HasProperty("_RippleStrength"))
        {
            float timeElapsed = 0f;
            
            while (timeElapsed < m_FadeDuration)
            {
                // 从设定的波纹强度到0，逐渐关闭效果
                float effectValue = Mathf.Lerp(m_RippleStrength, 0, timeElapsed / m_FadeDuration);
                m_BlackoutImage.material.SetFloat("_RippleStrength", effectValue);
                
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            
            // 设置最终值为0，完全关闭效果
            m_BlackoutImage.material.SetFloat("_RippleStrength", 0f);
            
            // 设置图像为透明
            m_BlackoutImage.color = new Color(1, 1, 1, 1);
        }
        else
        {
            // 等待相同的时间，以保持流程一致
            yield return new WaitForSeconds(m_FadeDuration);
            
            // 设置图像为透明
            m_BlackoutImage.color = new Color(1, 1, 1, 1);
        }
    }
    
    /// <summary>
    /// 禁用玩家控制
    /// </summary>
    private void DisablePlayerControl()
    {
        if (m_PlayerObject == null) return;
        
        PlayerControlManager controlManager = m_PlayerObject.GetComponent<PlayerControlManager>();
        if (controlManager != null)
        {
            controlManager.DisableControl();
        }
    }
    
    /// <summary>
    /// 启用玩家控制
    /// </summary>
    private void EnablePlayerControl()
    {
        if (m_PlayerObject == null) return;
        
        PlayerControlManager controlManager = m_PlayerObject.GetComponent<PlayerControlManager>();
        if (controlManager != null)
        {
            controlManager.EnableControl();
        }
    }

    /// <summary>
    /// 获取波纹材质
    /// </summary>
    /// <returns>波纹材质</returns>
    public Material GetRippleMaterial()
    {
        return m_RippleMaterial;
    }
}

