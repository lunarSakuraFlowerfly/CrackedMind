using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Characters.Player
{
    /// <summary>
    /// 淡入淡出管理器，用于实现传送时的淡入淡出效果
    /// </summary>
    public class FadeManager : MonoBehaviour
    {


        #region Singleton
        private static FadeManager s_Instance;
        public static FadeManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindObjectOfType<FadeManager>();
                    if (s_Instance == null)
                    {
                        GameObject go = new GameObject("FadeManager");
                        s_Instance = go.AddComponent<FadeManager>();
                    }
                }
                return s_Instance;
            }
        }
        #endregion

        #region Private Fields
        [Header("淡入淡出设置")]
        [Tooltip("淡入淡出面板")]
        [SerializeField] private Image m_FadePanel;
        
        [Tooltip("淡入淡出颜色")]
        [SerializeField] private Color m_FadeColor = Color.black;
        
        [Tooltip("淡入淡出时间")]
        [SerializeField] private float m_FadeTime = 0.5f;
        
        [Tooltip("淡入淡出曲线")]
        [SerializeField] private AnimationCurve m_FadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        private bool m_IsFading = false;
        private Coroutine m_FadeCoroutine;
        private Canvas m_FadeCanvas;
        private GraphicRaycaster m_Raycaster;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            s_Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 如果没有设置淡入淡出面板，创建一个
            if (m_FadePanel == null)
            {
                CreateFadePanel();
            }

            Color color = m_FadePanel.color;
            color.a = 0;
            m_FadePanel.color = color;
        }
        
        private void OnEnable()
        {
            // 确保淡入淡出面板的raycastTarget设置为false，这样它就不会拦截UI事件
            if (m_FadePanel != null)
            {
                m_FadePanel.raycastTarget = false;
            }
            
            // 确保GraphicRaycaster被禁用，这样整个淡入淡出Canvas就不会拦截UI事件
            if (m_Raycaster != null)
            {
                m_Raycaster.enabled = false;
            }
        }

        private void Start()
        {
            FadeOut();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 淡入效果
        /// </summary>
        /// <param name="_callback">淡入完成后的回调</param>
        public void FadeIn(System.Action _callback = null)
        {
            Debug.Log("FadeManager: 开始淡入效果");
            
            if (m_IsFading)
            {
                Debug.Log("FadeManager: 已有淡入淡出效果在进行中，停止当前效果");
                if (m_FadeCoroutine != null)
                {
                    StopCoroutine(m_FadeCoroutine);
                }
            }
            
            m_FadeCoroutine = StartCoroutine(FadeCoroutine(0, 1, () => {
                Debug.Log("FadeManager: 淡入效果完成，执行回调");
                _callback?.Invoke();
            }));
        }
        
        /// <summary>
        /// 淡出效果
        /// </summary>
        /// <param name="_callback">淡出完成后的回调</param>
        public void FadeOut(System.Action _callback = null)
        {
            Debug.Log("FadeManager: 开始淡出效果");
            
            if (m_IsFading)
            {
                Debug.Log("FadeManager: 已有淡入淡出效果在进行中，停止当前效果");
                if (m_FadeCoroutine != null)
                {
                    StopCoroutine(m_FadeCoroutine);
                }
            }
            
            m_FadeCoroutine = StartCoroutine(FadeCoroutine(1, 0, () => {
                Debug.Log("FadeManager: 淡出效果完成，执行回调");
                _callback?.Invoke();
            }));
        }
        
        /// <summary>
        /// 淡入淡出效果
        /// </summary>
        /// <param name="_callback">淡入淡出完成后的回调</param>
        public void FadeInOut(System.Action _callback = null)
        {
            Debug.Log("FadeManager: 开始淡入淡出效果");
            
            if (m_IsFading)
            {
                Debug.Log("FadeManager: 已有淡入淡出效果在进行中，停止当前效果");
                if (m_FadeCoroutine != null)
                {
                    StopCoroutine(m_FadeCoroutine);
                }
            }
            
            m_FadeCoroutine = StartCoroutine(FadeInOutCoroutine(() => {
                Debug.Log("FadeManager: 淡入淡出效果完成，执行回调");
                _callback?.Invoke();
            }));
        }
        
        /// <summary>
        /// 设置淡入淡出颜色
        /// </summary>
        public void SetFadeColor(Color _color)
        {
            m_FadeColor = _color;
            if (m_FadePanel != null)
            {
                m_FadePanel.color = new Color(m_FadeColor.r, m_FadeColor.g, m_FadeColor.b, m_FadePanel.color.a);
            }
        }
        
        /// <summary>
        /// 设置淡入淡出时间
        /// </summary>
        public void SetFadeTime(float _time)
        {
            m_FadeTime = _time;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 创建淡入淡出面板
        /// </summary>
        private void CreateFadePanel()
        {
            // 创建Canvas
            GameObject canvasObj = new GameObject("FadeCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999; // 确保在最上层
            
            // 添加CanvasScaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // 添加GraphicRaycaster，但默认禁用它
            GraphicRaycaster raycaster = canvasObj.AddComponent<GraphicRaycaster>();
            raycaster.enabled = false; // 禁用射线检测，这样就不会拦截UI事件
            m_Raycaster = raycaster;
            
            // 创建面板
            GameObject panelObj = new GameObject("FadePanel");
            panelObj.transform.SetParent(canvasObj.transform, false);
            
            // 添加Image组件
            Image image = panelObj.AddComponent<Image>();
            image.color = new Color(m_FadeColor.r, m_FadeColor.g, m_FadeColor.b, 0);
            image.raycastTarget = false; // 禁用射线检测，这样就不会拦截UI事件
            
            // 设置RectTransform
            RectTransform rectTransform = image.rectTransform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            
            // 设置淡入淡出面板
            m_FadePanel = image;
            m_FadeCanvas = canvas;
            
            // 将Canvas设置为FadeManager的子物体
            canvasObj.transform.SetParent(transform);
        }
        
        /// <summary>
        /// 淡入淡出协程
        /// </summary>
        private IEnumerator FadeCoroutine(float _startAlpha, float _endAlpha, System.Action _callback)
        {
            Debug.Log($"FadeManager: 开始淡入淡出协程，从 {_startAlpha} 到 {_endAlpha}");
            m_IsFading = true;
            
            float elapsedTime = 0;
            Color color = m_FadePanel.color;
            
            while (elapsedTime < m_FadeTime)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / m_FadeTime;
                t = m_FadeCurve.Evaluate(t);
                
                color.a = Mathf.Lerp(_startAlpha, _endAlpha, t);
                m_FadePanel.color = color;
                
                yield return null;
            }
            
            color.a = _endAlpha;
            m_FadePanel.color = color;
            
            m_IsFading = false;
            Debug.Log($"FadeManager: 淡入淡出协程完成，当前透明度: {color.a}");
            _callback?.Invoke();
        }
        
        /// <summary>
        /// 淡入淡出协程
        /// </summary>
        private IEnumerator FadeInOutCoroutine(System.Action _callback)
        {
            Debug.Log("FadeManager: 开始淡入淡出协程");
            m_IsFading = true;
            
            // 淡入
            float elapsedTime = 0;
            Color color = m_FadePanel.color;
            
            while (elapsedTime < m_FadeTime)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / m_FadeTime;
                t = m_FadeCurve.Evaluate(t);
                
                color.a = Mathf.Lerp(0, 1, t);
                m_FadePanel.color = color;
                
                yield return null;
            }
            
            color.a = 1;
            m_FadePanel.color = color;
            Debug.Log("FadeManager: 淡入完成，当前透明度: 1");
            
            // 淡出
            elapsedTime = 0;
            
            while (elapsedTime < m_FadeTime)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / m_FadeTime;
                t = m_FadeCurve.Evaluate(t);
                
                color.a = Mathf.Lerp(1, 0, t);
                m_FadePanel.color = color;
                
                yield return null;
            }
            
            color.a = 0;
            m_FadePanel.color = color;
            
            m_IsFading = false;
            Debug.Log("FadeManager: 淡出完成，当前透明度: 0");
            _callback?.Invoke();
        }
        #endregion
    }
} 