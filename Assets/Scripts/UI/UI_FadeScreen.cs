using UnityEngine;
using System.Collections;

public class UI_FadeScreen : MonoBehaviour
{
    private Animator anim;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("UI_FadeScreen: 未找到Animator组件！");
        }
    }

    public void FadeOut()
    {
        if (anim != null)
        {
            Debug.Log("UI_FadeScreen: 触发FadeOut动画");
            anim.SetTrigger("FadeOut");
        }
        else
        {
            Debug.LogError("UI_FadeScreen: Animator为空，无法播放FadeOut");
            // 重新获取Animator组件
            anim = GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("FadeOut");
            }
        }
    }
    
    public void FadeIn()
    {
        if (anim != null)
        {
            Debug.Log("UI_FadeScreen: 触发FadeIn动画");
            anim.SetTrigger("FadeIn");
        }
        else
        {
            Debug.LogError("UI_FadeScreen: Animator为空，无法播放FadeIn");
            // 重新获取Animator组件
            anim = GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("FadeIn");
            }
        }
    }
    
    // 添加一个方法来检查动画状态
    public bool IsAnimating()
    {
        if (anim == null) return false;
        
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime < 1.0f;
    }
}