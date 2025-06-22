using UnityEngine;
using UnityEditor;
using TMPro;

public class FontReplacer : MonoBehaviour
{
    [MenuItem("Tools/替换所有TMP字体")]
    public static void ReplaceAllTMPFonts()
    {
        // 选择新的字体资源
        TMP_FontAsset newFont = Selection.activeObject as TMP_FontAsset;
        if (newFont == null)
        {
            EditorUtility.DisplayDialog("提示", "请在 Project 中选中一个 TMP_FontAsset 字体资源", "确定");
            return;
        }

        // 获取场景中所有 TMP 组件
        TextMeshProUGUI[] uiTexts = GameObject.FindObjectsOfType<TextMeshProUGUI>(true);
        TextMeshPro[] worldTexts = GameObject.FindObjectsOfType<TextMeshPro>(true);

        int count = 0;

        foreach (var text in uiTexts)
        {
            if (text.font != newFont)
            {
                Undo.RecordObject(text, "Change TMP Font");
                text.font = newFont;
                count++;
            }
        }

        foreach (var text in worldTexts)
        {
            if (text.font != newFont)
            {
                Undo.RecordObject(text, "Change TMP Font");
                text.font = newFont;
                count++;
            }
        }

        EditorUtility.DisplayDialog("完成", $"共替换 {count} 个 TextMeshPro 组件字体", "好");
    }
}
