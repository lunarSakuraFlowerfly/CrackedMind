using UnityEngine;
using TMPro;
using System.Collections;
using UnityEditor;

namespace Characters.Player
{
    /// <summary>
    /// 传送触发组件，当玩家进入触发区域时传送到指定位置
    /// </summary>
    public class TeleportTrigger : MonoBehaviour
    {
        [Header("传送设置")]
        [Tooltip("传送触发区域ID")]
        [SerializeField] private string m_TriggerID;
        
        [Tooltip("目标传送点ID")]
        [SerializeField] private string m_TargetTriggerID;
        
        [Tooltip("交互按键")]
        [SerializeField] private KeyCode m_InteractKey = KeyCode.F;
        
        [Tooltip("传送时是否保持朝向")]
        [SerializeField] private bool m_MaintainRotation = true;
        
        [Tooltip("传送延迟时间")]
        [SerializeField] private float m_TeleportDelay = 0.5f;
        
        [Tooltip("传送冷却时间")]
        [SerializeField] private float m_CooldownTime = 3f;
        
        [Header("碰撞器设置")]
        [Tooltip("碰撞器形状")]
        [SerializeField] private ColliderShape m_ColliderShape = ColliderShape.Circle;
        
        [Tooltip("圆形碰撞器半径")]
        [SerializeField] private float m_InteractionRange = 2f;
        
        [Tooltip("方形碰撞器宽度")]
        [SerializeField] private float m_BoxWidth = 2f;
        
        [Tooltip("方形碰撞器高度")]
        [SerializeField] private float m_BoxHeight = 2f;
        
        [Header("特效设置")]
        [Tooltip("传送开始特效")]
        [SerializeField] private GameObject m_TeleportStartEffect;
        
        [Tooltip("传送结束特效")]
        [SerializeField] private GameObject m_TeleportEndEffect;
        
        [Header("提示设置")]
        [Tooltip("是否显示提示")]
        [SerializeField] private bool m_ShowPrompt = true;
        
        [Tooltip("提示文本")]
        [SerializeField] private string m_PromptText = "按E传送";
        
        [Tooltip("提示UI预制体")]
        [SerializeField] private GameObject m_PromptPrefab;
        
        private bool m_IsPlayerInRange = false;
        private GameObject m_Player;
        private GameObject m_PromptInstance;
        private TextMeshProUGUI m_PromptTextComponent;
        private bool m_CanTeleport = true;
        private bool m_IsTeleporting = false;
        private CameraController m_CameraController;
        private Collider2D m_Collider;
        private PlayerMovement m_PlayerMovement;
        
        /// <summary>
        /// 碰撞器形状枚举
        /// </summary>
        public enum ColliderShape
        {
            Circle,
            Box
        }
        
        private void Start()
        {
            // 创建提示UI
            if (m_ShowPrompt && m_PromptPrefab != null)
            {
                m_PromptInstance = Instantiate(m_PromptPrefab, transform);
                m_PromptTextComponent = m_PromptInstance.GetComponentInChildren<TextMeshProUGUI>();
                if (m_PromptTextComponent != null)
                {
                    m_PromptTextComponent.text = m_PromptText;
                }
                m_PromptInstance.SetActive(false);
            }
            
            // 创建碰撞器
            CreateCollider();
            
            // 验证设置
            ValidateSettings();
        }
        
        /// <summary>
        /// 创建碰撞器
        /// </summary>
        private void CreateCollider()
        {
            // 移除现有碰撞器
            Collider2D existingCollider = GetComponent<Collider2D>();
            if (existingCollider != null)
            {
                DestroyImmediate(existingCollider);
            }
            
            // 根据选择的形状创建碰撞器
            switch (m_ColliderShape)
            {
                case ColliderShape.Circle:
                    CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
                    circleCollider.isTrigger = true;
                    circleCollider.radius = m_InteractionRange;
                    m_Collider = circleCollider;
                    break;
                    
                case ColliderShape.Box:
                    BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
                    boxCollider.isTrigger = true;
                    boxCollider.size = new Vector2(m_BoxWidth, m_BoxHeight);
                    m_Collider = boxCollider;
                    break;
            }
        }
        
        private void Update()
        {
            // 如果玩家在范围内且按下交互键
            if (m_IsPlayerInRange && Input.GetKeyDown(m_InteractKey) && m_CanTeleport && !m_IsTeleporting)
            {
                StartCoroutine(TeleportCoroutine());
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            // 检查是否是玩家
            if (other.CompareTag("Player"))
            {
                m_IsPlayerInRange = true;
                m_Player = other.gameObject;
                
                // 获取玩家移动组件
                m_PlayerMovement = m_Player.GetComponent<PlayerMovement>();
                
                // 获取摄像机控制器
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    m_CameraController = mainCamera.GetComponent<CameraController>();
                }
                
                // 显示提示
                if (m_ShowPrompt && m_PromptInstance != null)
                {
                    m_PromptInstance.SetActive(true);
                }
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            // 检查是否是玩家
            if (other.CompareTag("Player"))
            {
                m_IsPlayerInRange = false;
                m_Player = null;
                
                // 隐藏提示
                if (m_PromptInstance != null)
                {
                    m_PromptInstance.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// 传送协程
        /// </summary>
        private IEnumerator TeleportCoroutine()
        {
            m_IsTeleporting = true;
            Debug.Log("开始传送协程");
            
            // 确保m_Player引用有效
            if (m_Player == null)
            {
                Debug.LogWarning("m_Player引用丢失，尝试重新获取");
                m_Player = GameObject.FindGameObjectWithTag("Player");
                
                if (m_Player == null)
                {
                    Debug.LogError("无法获取Player对象，传送中止");
                    m_IsTeleporting = false;
                    yield break;
                }
                
                // 重新获取PlayerMovement组件
                m_PlayerMovement = m_Player.GetComponent<PlayerMovement>();
            }
            
            // 禁用玩家移动
            if (m_PlayerMovement != null)
            {
                Debug.Log("禁用玩家移动");
                m_PlayerMovement.SetMovementEnabled(false);
            }
            else
            {
                Debug.LogWarning("未找到PlayerMovement组件，无法禁用移动");
            }
            
            // 保存当前位置和朝向
            Vector3 startPosition = m_Player.transform.position;
            Quaternion startRotation = m_Player.transform.rotation;
            
            // 播放开始特效
            if (m_TeleportStartEffect != null)
            {
                Instantiate(m_TeleportStartEffect, startPosition, Quaternion.identity);
            }
            
            // 等待传送延迟
            yield return new WaitForSeconds(m_TeleportDelay);
            
            // 查找目标传送点
            TeleportTrigger targetTrigger = FindTeleportTrigger(m_TargetTriggerID);
            
            if (targetTrigger != null)
            {
                Debug.Log($"找到目标传送点: {targetTrigger.GetTriggerID()}");
                
                // 保存玩家当前朝向
                Quaternion playerRotation = m_Player.transform.rotation;
                
                // 使用淡入淡出效果进行传送
                bool fadeInComplete = false;
                bool fadeOutComplete = false;
                
                Debug.Log("开始淡入效果");
                
                // 淡入效果
                FadeManager.Instance.FadeIn(() => {
                    Debug.Log("淡入完成");
                    fadeInComplete = true;
                    
                    // 直接设置玩家位置
                    m_Player.transform.position = targetTrigger.transform.position;
                    Debug.Log($"玩家已传送到: {targetTrigger.transform.position}");
                    
                    // 如果需要保持朝向，使用玩家原来的朝向
                    if (m_MaintainRotation)
                    {
                        m_Player.transform.rotation = playerRotation;
                    }
                    
                    // 播放结束特效
                    if (m_TeleportEndEffect != null)
                    {
                        Instantiate(m_TeleportEndEffect, m_Player.transform.position, Quaternion.identity);
                    }
                    
                    // 通知摄像机更新
                    NotifyCameraUpdate();
                    
                    Debug.Log("开始淡出效果");
                    
                    // 淡出效果
                    FadeManager.Instance.FadeOut(() => {
                        Debug.Log("淡出完成");
                        fadeOutComplete = true;
                    });
                });
                
                // 等待淡入淡出完成
                Debug.Log("等待淡入淡出完成");
                while (!fadeInComplete || !fadeOutComplete)
                {
                    yield return null;
                }
                
                Debug.Log("淡入淡出完成");
                
                // 重新启用玩家移动
                if (m_PlayerMovement != null)
                {
                    Debug.Log("重新启用玩家移动");
                    m_PlayerMovement.SetMovementEnabled(true);
                }
                
                // 开始冷却
                m_CanTeleport = false;
                StartCoroutine(CooldownCoroutine());
            }
            else
            {
                Debug.LogWarning($"未找到目标传送点: {m_TargetTriggerID}");
                
                // 重新启用玩家移动
                if (m_PlayerMovement != null)
                {
                    Debug.Log("重新启用玩家移动");
                    m_PlayerMovement.SetMovementEnabled(true);
                }
                
                m_IsTeleporting = false;
            }
        }
        
        /// <summary>
        /// 冷却协程
        /// </summary>
        private IEnumerator CooldownCoroutine()
        {
            yield return new WaitForSeconds(m_CooldownTime);
            m_CanTeleport = true;
            m_IsTeleporting = false;
        }
        
        /// <summary>
        /// 通知摄像机更新位置
        /// </summary>
        private void NotifyCameraUpdate()
        {
            // 查找主摄像机
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                // 如果有CameraController组件，直接调用其方法
                if (m_CameraController != null)
                {
                    // 使用CameraController的UpdatePosition方法立即更新摄像机位置
                    m_CameraController.UpdatePosition();
                }
                else
                {
                    // 尝试使用反射调用摄像机控制器的更新方法
                    MonoBehaviour[] components = mainCamera.GetComponents<MonoBehaviour>();
                    foreach (MonoBehaviour component in components)
                    {
                        // 尝试调用UpdatePosition方法
                        System.Reflection.MethodInfo updateMethod = component.GetType().GetMethod("UpdatePosition");
                        if (updateMethod != null)
                        {
                            updateMethod.Invoke(component, null);
                            break;
                        }
                        
                        // 尝试调用UpdateCamera方法
                        updateMethod = component.GetType().GetMethod("UpdateCamera");
                        if (updateMethod != null)
                        {
                            updateMethod.Invoke(component, null);
                            break;
                        }
                        
                        // 尝试调用ResetPosition方法
                        updateMethod = component.GetType().GetMethod("ResetPosition");
                        if (updateMethod != null)
                        {
                            updateMethod.Invoke(component, null);
                            break;
                        }
                    }
                    
                    // 如果没有找到任何更新方法，直接设置摄像机位置
                    mainCamera.transform.position = m_Player.transform.position + new Vector3(0, 0, -10);
                }
            }
        }
        
        /// <summary>
        /// 查找指定ID的传送点
        /// </summary>
        private TeleportTrigger FindTeleportTrigger(string triggerID)
        {
            TeleportTrigger[] triggers = FindObjectsOfType<TeleportTrigger>();
            foreach (TeleportTrigger trigger in triggers)
            {
                if (trigger.GetTriggerID() == triggerID)
                {
                    return trigger;
                }
            }
            return null;
        }
        
        /// <summary>
        /// 验证设置
        /// </summary>
        private void ValidateSettings()
        {
            // 确保ID不为空
            if (string.IsNullOrEmpty(m_TriggerID))
            {
                m_TriggerID = $"trigger_{gameObject.name}";
            }
            
            if (string.IsNullOrEmpty(m_TargetTriggerID))
            {
                Debug.LogWarning($"传送触发区域 {m_TriggerID} 的目标传送点ID未设置");
            }
        }
        
        /// <summary>
        /// 在编辑器中验证并更新设置
        /// </summary>
        private void OnValidate()
        {
            // 验证设置
            ValidateSettings();
            
            // 更新碰撞器
            if (Application.isPlaying)
            {
                // 运行时通过CreateCollider方法更新
                CreateCollider();
            }
            else
            {
                // 编辑器中直接更新现有碰撞器
                Collider2D existingCollider = GetComponent<Collider2D>();
                if (existingCollider != null)
                {
                    if (m_ColliderShape == ColliderShape.Circle && existingCollider is CircleCollider2D circleCollider)
                    {
                        circleCollider.radius = m_InteractionRange;
                    }
                    else if (m_ColliderShape == ColliderShape.Box && existingCollider is BoxCollider2D boxCollider)
                    {
                        boxCollider.size = new Vector2(m_BoxWidth, m_BoxHeight);
                    }
                }
                else
                {
                    // 如果没有碰撞器，创建一个新的
                    // 注意：在OnValidate中直接添加组件可能会导致问题
                    // 所以我们使用EditorApplication.delayCall来延迟执行
                    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.delayCall += () => {
                        if (this != null) // 确保对象仍然存在
                        {
                            CreateCollider();
                        }
                    };
                    #endif
                }
            }
        }
        
        /// <summary>
        /// 在编辑器中可视化触发区域
        /// </summary>
        private void OnDrawGizmos()
        {
            // 根据选择的形状绘制交互范围
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            
            switch (m_ColliderShape)
            {
                case ColliderShape.Circle:
                    Gizmos.DrawWireSphere(transform.position, m_InteractionRange);
                    break;
                    
                case ColliderShape.Box:
                    Gizmos.DrawWireCube(transform.position, new Vector3(m_BoxWidth, m_BoxHeight, 0));
                    break;
            }
            
            // 如果有目标点，绘制连线
            TeleportTrigger targetTrigger = FindTeleportTrigger(m_TargetTriggerID);
            if (targetTrigger != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, targetTrigger.transform.position);
                Gizmos.DrawWireSphere(targetTrigger.transform.position, 0.5f);
            }
        }
        
        /// <summary>
        /// 设置触发区域ID
        /// </summary>
        public void SetTriggerID(string id)
        {
            m_TriggerID = id;
        }
        
        /// <summary>
        /// 设置目标传送点ID
        /// </summary>
        public void SetTargetTriggerID(string id)
        {
            m_TargetTriggerID = id;
        }
        
        /// <summary>
        /// 获取触发区域ID
        /// </summary>
        public string GetTriggerID()
        {
            return m_TriggerID;
        }
        
        /// <summary>
        /// 设置碰撞器形状
        /// </summary>
        public void SetColliderShape(ColliderShape shape)
        {
            if (m_ColliderShape != shape)
            {
                m_ColliderShape = shape;
                CreateCollider();
            }
        }
        
        /// <summary>
        /// 设置交互范围
        /// </summary>
        public void SetInteractionRange(float range)
        {
            m_InteractionRange = range;
            if (m_ColliderShape == ColliderShape.Circle && m_Collider is CircleCollider2D circleCollider)
            {
                circleCollider.radius = range;
            }
        }
        
        /// <summary>
        /// 设置方形碰撞器尺寸
        /// </summary>
        public void SetBoxSize(float width, float height)
        {
            m_BoxWidth = width;
            m_BoxHeight = height;
            if (m_ColliderShape == ColliderShape.Box && m_Collider is BoxCollider2D boxCollider)
            {
                boxCollider.size = new Vector2(width, height);
            }
        }
    }
    
    /// <summary>
    /// 摄像机跟随脚本
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Tooltip("跟随目标")]
        [SerializeField] private Transform m_Target;
        
        [Tooltip("跟随速度")]
        [SerializeField] private float m_SmoothSpeed = 0.125f;
        
        [Tooltip("偏移量")]
        [SerializeField] private Vector3 m_Offset = new Vector3(0, 0, -10);
        
        private void LateUpdate()
        {
            if (m_Target == null) return;
            
            // 计算目标位置
            Vector3 desiredPosition = m_Target.position + m_Offset;
            
            // 平滑移动摄像机
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, m_SmoothSpeed);
            transform.position = smoothedPosition;
        }
        
        /// <summary>
        /// 立即更新摄像机位置
        /// </summary>
        public void UpdatePosition()
        {
            if (m_Target == null) return;
            
            // 立即设置摄像机位置
            transform.position = m_Target.position + m_Offset;
        }
    }
    
    #if UNITY_EDITOR
    /// <summary>
    /// TeleportTrigger的自定义Inspector
    /// </summary>
    [CustomEditor(typeof(TeleportTrigger))]
    public class TeleportTriggerEditor : Editor
    {
        private SerializedProperty m_TriggerID;
        private SerializedProperty m_TargetTriggerID;
        private SerializedProperty m_InteractKey;
        private SerializedProperty m_MaintainRotation;
        private SerializedProperty m_TeleportDelay;
        private SerializedProperty m_CooldownTime;
        private SerializedProperty m_ColliderShape;
        private SerializedProperty m_InteractionRange;
        private SerializedProperty m_BoxWidth;
        private SerializedProperty m_BoxHeight;
        private SerializedProperty m_TeleportStartEffect;
        private SerializedProperty m_TeleportEndEffect;
        private SerializedProperty m_ShowPrompt;
        private SerializedProperty m_PromptText;
        private SerializedProperty m_PromptPrefab;
        
        private void OnEnable()
        {
            m_TriggerID = serializedObject.FindProperty("m_TriggerID");
            m_TargetTriggerID = serializedObject.FindProperty("m_TargetTriggerID");
            m_InteractKey = serializedObject.FindProperty("m_InteractKey");
            m_MaintainRotation = serializedObject.FindProperty("m_MaintainRotation");
            m_TeleportDelay = serializedObject.FindProperty("m_TeleportDelay");
            m_CooldownTime = serializedObject.FindProperty("m_CooldownTime");
            m_ColliderShape = serializedObject.FindProperty("m_ColliderShape");
            m_InteractionRange = serializedObject.FindProperty("m_InteractionRange");
            m_BoxWidth = serializedObject.FindProperty("m_BoxWidth");
            m_BoxHeight = serializedObject.FindProperty("m_BoxHeight");
            m_TeleportStartEffect = serializedObject.FindProperty("m_TeleportStartEffect");
            m_TeleportEndEffect = serializedObject.FindProperty("m_TeleportEndEffect");
            m_ShowPrompt = serializedObject.FindProperty("m_ShowPrompt");
            m_PromptText = serializedObject.FindProperty("m_PromptText");
            m_PromptPrefab = serializedObject.FindProperty("m_PromptPrefab");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // 传送设置
            EditorGUILayout.LabelField("传送设置", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_TriggerID, new GUIContent("传送触发区域ID"));
            EditorGUILayout.PropertyField(m_TargetTriggerID, new GUIContent("目标传送点ID"));
            EditorGUILayout.PropertyField(m_InteractKey, new GUIContent("交互按键"));
            EditorGUILayout.PropertyField(m_MaintainRotation, new GUIContent("传送时是否保持朝向"));
            EditorGUILayout.PropertyField(m_TeleportDelay, new GUIContent("传送延迟时间"));
            EditorGUILayout.PropertyField(m_CooldownTime, new GUIContent("传送冷却时间"));
            
            EditorGUILayout.Space();
            
            // 碰撞器设置
            EditorGUILayout.LabelField("碰撞器设置", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_ColliderShape, new GUIContent("碰撞器形状"));
            
            // 根据选择的碰撞器形状显示不同的设置
            TeleportTrigger.ColliderShape colliderShape = (TeleportTrigger.ColliderShape)m_ColliderShape.enumValueIndex;
            
            if (colliderShape == TeleportTrigger.ColliderShape.Circle)
            {
                EditorGUILayout.PropertyField(m_InteractionRange, new GUIContent("圆形碰撞器半径"));
            }
            else if (colliderShape == TeleportTrigger.ColliderShape.Box)
            {
                EditorGUILayout.PropertyField(m_BoxWidth, new GUIContent("方形碰撞器宽度"));
                EditorGUILayout.PropertyField(m_BoxHeight, new GUIContent("方形碰撞器高度"));
            }
            
            EditorGUILayout.Space();
            
            // 特效设置
            EditorGUILayout.LabelField("特效设置", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_TeleportStartEffect, new GUIContent("传送开始特效"));
            EditorGUILayout.PropertyField(m_TeleportEndEffect, new GUIContent("传送结束特效"));
            
            EditorGUILayout.Space();
            
            // 提示设置
            EditorGUILayout.LabelField("提示设置", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_ShowPrompt, new GUIContent("是否显示提示"));
            EditorGUILayout.PropertyField(m_PromptText, new GUIContent("提示文本"));
            EditorGUILayout.PropertyField(m_PromptPrefab, new GUIContent("提示UI预制体"));
            
            serializedObject.ApplyModifiedProperties();
            
            // 如果属性发生变化，调用OnValidate方法
            if (GUI.changed)
            {
                TeleportTrigger teleportTrigger = (TeleportTrigger)target;
                // 使用更安全的方式调用OnValidate方法
                #if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    teleportTrigger.SendMessage("OnValidate", null, SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    // 在编辑器中直接调用OnValidate方法
                    System.Reflection.MethodInfo validateMethod = typeof(TeleportTrigger).GetMethod("OnValidate", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (validateMethod != null)
                    {
                        validateMethod.Invoke(teleportTrigger, null);
                    }
                }
                #endif
            }
        }
    }
    #endif
} 