using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_SaveAndExit : MonoBehaviour
{
    public void SaveAndExit()
    {
        StartCoroutine(LoadMenuAfterSave());
    }

    private IEnumerator LoadMenuAfterSave()
    {
        // 等待一帧确保保存操作完成
        yield return null;
        
        // 切换到菜单场景
        GameManager.Instance.ResumeGame();
        AudioManager.instance.PlayBGM(1);
        SaveManager.instance.SaveGame();
        SceneManager.LoadScene("Main Menu");
        
    }
}
