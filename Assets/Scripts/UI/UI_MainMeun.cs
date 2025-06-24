using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "Dream";
    [SerializeField] private GameObject continueButton;
    [SerializeField] UI_FadeScreen fadeScreen;

    private void Start()
    {
        if(SaveManager.instance.HasSaveData()==false)
        {
            continueButton.SetActive(false);
        }
    }

    public void ContinueGame()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(4, null);
        
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    public void NewGame()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(4, null);
            
        SaveManager.instance.DeleteSaveData();
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    public void ExitGame()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(4, null);
            
        Debug.Log("exit game");
        Application.Quit();
    }

    IEnumerator LoadSceneWithFadeEffect(float _delay)
    {
        fadeScreen.FadeIn();
        yield return new WaitForSeconds(_delay);
        SceneManager.LoadScene(sceneName);
    }
}