using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemDataSO itemData;
    private Vector2 velocity;


    private void OnValidate()
    {
        if(itemData == null) return;
        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item object - " + itemData.itemName;
    }


    
    public void Pickup()
    {
        if(PlayerManager.instance.player.GetComponent<PlayerStats>().isDead) return;
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }

    public void SetupItem(ItemDataSO _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        velocity = _velocity;
        rb.velocity = velocity;
        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item object - " + itemData.itemName;
    }
    public void SetVelocity(Vector2 _velocity)
    {
        rb.velocity = _velocity;
    }
}

