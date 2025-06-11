using UnityEngine;
using UnityEditor;

public class RippleShaderGUI : ShaderGUI
{
    /// <summary>
    /// 自定义Ripple Shader的GUI
    /// </summary>
    /// <param name="materialEditor">材质编辑器</param>
    /// <param name="properties">属性数组</param>
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // 查找属性
        MaterialProperty mainTex = FindProperty("_MainTex", properties);
        MaterialProperty rippleStrength = FindProperty("_RippleStrength", properties);
        MaterialProperty rippleSpeed = FindProperty("_RippleSpeed", properties);
        MaterialProperty rippleSize = FindProperty("_RippleSize", properties);
        MaterialProperty effectEnabled = FindProperty("_EffectEnabled", properties);
        MaterialProperty previewMode = FindProperty("_PreviewMode", properties);
        
        Material targetMat = materialEditor.target as Material;
        
        // 绘制标题
        EditorGUILayout.LabelField("波纹效果参数", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // 绘制主要属性
        materialEditor.TexturePropertySingleLine(new GUIContent("主纹理"), mainTex);
        
        // 绘制预览模式开关
        materialEditor.ShaderProperty(previewMode, "预览模式");
        EditorGUILayout.HelpBox("开启预览模式可在材质预览窗口中查看效果变化", MessageType.Info);
        
        EditorGUILayout.Space();
        
        // 绘制效果属性
        materialEditor.ShaderProperty(effectEnabled, "启用效果");
        materialEditor.RangeProperty(rippleStrength, "波纹强度");
        materialEditor.RangeProperty(rippleSpeed, "波纹速度");
        materialEditor.RangeProperty(rippleSize, "波纹大小");
        
        // 提示信息
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("提示：调整参数可以在预览窗口中实时查看效果", MessageType.Info);
        
        // 绘制预览图示
        EditorGUILayout.Space();
        Rect previewRect = EditorGUILayout.GetControlRect(false, 200);
        if (Event.current.type == EventType.Repaint)
        {
            // 确保材质预览始终刷新
            if (previewMode.floatValue > 0)
            {
                EditorUtility.SetDirty(targetMat);
            }
        }
        
        // 添加测试按钮
        EditorGUILayout.Space();
        if (GUILayout.Button("测试波纹效果"))
        {
            // 临时设置最大值测试效果
            targetMat.SetFloat("_EffectEnabled", 1.0f);
            targetMat.SetFloat("_RippleStrength", 1.0f);
            targetMat.SetFloat("_RippleSpeed", 5.0f);
            targetMat.SetFloat("_RippleSize", 8.0f);
            
            // 5秒后恢复
            EditorUtility.DisplayDialog("波纹效果测试", "已临时设置最大效果值，可在场景中查看效果", "确定");
        }
    }
}