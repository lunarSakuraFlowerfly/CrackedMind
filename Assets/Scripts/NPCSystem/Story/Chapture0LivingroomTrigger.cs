using UnityEngine;
using NPCSystem.Core;
using UnityEngine.UIElements;
using NPCSystem.Story;

public class Chapture0LivingroomTrigger:MonoBehaviour
{
    private Collider2D EventTriggercollider;
    public void Start()
    {
        EventTriggercollider = GetComponent<Collider2D>();
        if(EventTriggercollider)
        {
            EventTriggercollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否为玩家
        if (other.CompareTag("Player"))
        {
            // 触发事件
            if (StoryManager.Instance != null)
            {
                StoryManager.Instance.Capture0NeighborFirstEnterBedroom();
            }
            
            // 销毁组件
    
            Destroy(this);
        }
    }
}

