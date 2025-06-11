using UnityEngine;


public class GameManager : MonoBehaviour
{
    // 单例模式
    public static GameManager Instance { get; private set; }
    

    
    // 游戏全局状态
    public bool isGamePaused { get; private set; }
    public float gameTime { get; private set; }

    


    private void Awake()
    {
        // 单例实现
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 场景切换时保留
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }
    
    
    // 游戏控制方法
    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }

    
    // 更新游戏时间
    private void Update()
    {
        if (!isGamePaused)
            gameTime += Time.deltaTime;
    }
}

