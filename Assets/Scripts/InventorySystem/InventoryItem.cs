using System;
[Serializable]
public class InventoryItem
{
    public ItemDataSO data;
    public int stackSize;
    public InventoryItem(ItemDataSO _data)
    {
        data = _data;
        AddStack();
    }

    public void AddStack()=>stackSize++;
    public void RemoveStack()=>stackSize--;
}
