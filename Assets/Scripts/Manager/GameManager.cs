using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour,ISaveManager
{
    // 单例模式
    public static GameManager Instance { get; private set; }
    [SerializeField] private Checkpoint[] checkpoints;
    [SerializeField] private Player player;
    [SerializeField] private string closestCheckpointLoaded;

    
    // 游戏全局状态
    public bool isGamePaused { get; private set; }
    public float gameTime { get; private set; }


    public void RestartScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
    
    private void Awake()
    {
        // 单例实现
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        checkpoints = FindObjectsOfType<Checkpoint>();
    }

    private void Start()
    {
        player = PlayerManager.instance.player;
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

    public void SaveData(ref GameData _data)
    {
        _data.closestCheckpointId = FindClosestCheckpoint()?.id;
        _data.checkpoints.Clear();
        foreach(Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.id,checkpoint.activated);
        }
    }

    public void LoadData(GameData _data)
    {
        foreach(var pair in _data.checkpoints)
        {
            foreach(Checkpoint checkpoint in checkpoints)
            {
                if(checkpoint.id==pair.Key&&pair.Value==true)
                {
                    checkpoint.ActiveCheckpoint();
                }
            }
        }

        closestCheckpointLoaded = _data.closestCheckpointId;
        Invoke("PlacePlayerAtClosestCheckpoint",0.2f);
    }

    private void PlacePlayerAtClosestCheckpoint()
    {
        foreach(Checkpoint checkpoint in checkpoints)
        {
            if(closestCheckpointLoaded==checkpoint.id)
            {
                PlayerManager.instance.player.transform.position = checkpoint.transform.position;
            }
        }
    }

    public Checkpoint FindClosestCheckpoint()
    {
        float closestDistance = Mathf.Infinity;
        Checkpoint closestCheckpoint = null;
        foreach(Checkpoint checkpoint in checkpoints)
        {
            float distance = Vector2.Distance(checkpoint.transform.position,player.transform.position);
            if(distance<closestDistance&&checkpoint.activated==true)
            {
                closestDistance = distance;
                closestCheckpoint = checkpoint;
            }
        }
        return closestCheckpoint;
    }


    

}

