using UnityEngine;

/// <summary>
/// 波纹效果测试工具
/// </summary>
public class RippleEffectTester : MonoBehaviour
{
    [SerializeField] private SleepWakeEffect m_SleepEffect;
    [SerializeField] private RipplePostProcess m_RipplePostProcess;
    
    [Header("测试参数")]
    [SerializeField] private float m_TestDuration = 2f;
    [SerializeField] private float m_TestStrength = 0.8f;
    [SerializeField] private float m_TestSpeed = 3f;
    [SerializeField] private float m_TestSize = 5f;
    
    private Material m_RippleMaterial;
    private bool m_IsTesting = false;
    
    private void Start()
    {
        // 如果没有设置，则自动查找组件
        if (m_SleepEffect == null)
        {
            m_SleepEffect = FindObjectOfType<SleepWakeEffect>();
        }
        
        if (m_RipplePostProcess == null && Camera.main != null)
        {
            m_RipplePostProcess = Camera.main.GetComponent<RipplePostProcess>();
        }
        
        if (m_SleepEffect != null)
        {
            m_RippleMaterial = m_SleepEffect.GetRippleMaterial();
        }
    }
    
    /// <summary>
    /// 测试波纹效果
    /// </summary>
    public void TestRippleEffect()
    {
        if (m_IsTesting || m_RippleMaterial == null || m_RipplePostProcess == null)
        {
            return;
        }
        
        m_IsTesting = true;
        
        // 激活效果
        m_RipplePostProcess.EnableEffect();
        
        // 设置测试参数
        m_RippleMaterial.SetFloat("_RippleStrength", m_TestStrength);
        m_RippleMaterial.SetFloat("_RippleSpeed", m_TestSpeed);
        m_RippleMaterial.SetFloat("_RippleSize", m_TestSize);
        
        // 延迟禁用效果
        Invoke("DisableTest", m_TestDuration);
        
        Debug.Log("<color=green>波纹效果测试已启动</color>");
    }
    
    private void DisableTest()
    {
        if (m_RipplePostProcess != null)
        {
            m_RipplePostProcess.DisableEffect();
        }
        
        m_IsTesting = false;
        Debug.Log("<color=yellow>波纹效果测试已结束</color>");
    }
    
    /// <summary>
    /// 在编辑器中添加测试按钮
    /// </summary>
    private void OnGUI()
    {
        if (!Application.isEditor) return;
        
        if (GUI.Button(new Rect(10, 10, 150, 30), "测试波纹效果"))
        {
            TestRippleEffect();
        }
    }
}