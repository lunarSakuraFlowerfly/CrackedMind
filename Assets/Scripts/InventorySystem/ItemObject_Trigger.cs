using UnityEngine;

public class ItemObject_Trigger : MonoBehaviour
{
    private ItemObject itemObject => GetComponentInParent<ItemObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<Player>() != null)
        {
            
            itemObject.Pickup();
        }
    }
}
