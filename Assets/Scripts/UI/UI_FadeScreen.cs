using UnityEngine;
public class UI_FadeScreen : MonoBehaviour
{
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    public void FadeOut()=>anim.SetTrigger("FadeOut");
    public void FadeIn()=>anim.SetTrigger("FadeIn");
}