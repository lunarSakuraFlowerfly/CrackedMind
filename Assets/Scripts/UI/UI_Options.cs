using UnityEngine;
public class UI_Options : MonoBehaviour
{
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}