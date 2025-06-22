using System.Collections;
using System.Collections.Generic;
using InteractionSystem.Data;
using UnityEngine;
using UnityEditor;

public class Inventory : MonoBehaviour,ISaveManager
{
    public static Inventory instance;
    public List<ItemDataSO> startingItems;

    public List<InventoryItem> equipment;
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;
    public List<InventoryItem> inventory;
    public Dictionary<ItemDataSO, InventoryItem> inventoryDictionary;
    public List<InventoryItem> stash;
    public Dictionary<ItemDataSO, InventoryItem> stashDictionary;
    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;
    private UI_ItemSlot[] inventoryItemSlots;
    private UI_ItemSlot[] stashItemSlots;
    private UI_EquipmentSlot[] equipmentSlots;
    private UI_StatSlot[] statSlots;

    [Header("Items cooldown")]
    private float lastTimeUsedFlask;

    [Header("Data base")]

    public List<InventoryItem> loadedItems;
    public List<ItemData_Equipment> loadedEquipment;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemDataSO, InventoryItem>();
        inventoryItemSlots = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemDataSO, InventoryItem>();
        stashItemSlots = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();
        equipmentSlots = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();
        statSlots = statSlotParent.GetComponentsInChildren<UI_StatSlot>();

        AddStartingItems();
    }
    private void UpdateSlotUI()
    {
        for(int i = 0; i < equipmentSlots.Length; i++)
        {
            foreach(var item in equipmentDictionary)
            {
                if(item.Key.equipmentType == equipmentSlots[i].slotType)
                {
                    equipmentSlots[i].UpdateSlot(item.Value);
                }
            }
        }
        for(int i = 0; i < inventoryItemSlots.Length; i++)
        {
            inventoryItemSlots[i].CleanUpSlot();
        }
        for(int i = 0; i < stashItemSlots.Length; i++)
        {
            stashItemSlots[i].CleanUpSlot();
        }
        for(int i = 0; i < inventory.Count; i++)
        {
            inventoryItemSlots[i].UpdateSlot(inventory[i]);
        }
        for(int i = 0; i < stash.Count; i++)
        {
            stashItemSlots[i].UpdateSlot(stash[i]);
        }
        for(int i = 0; i < statSlots.Length; i++)
        {
            statSlots[i].UpdateStateValueUI();
        }
    }
    private void AddStartingItems()
    {
        foreach(var item in loadedEquipment)
        {
            EquipItem(item);
        }
        if(loadedItems.Count > 0)
        {
            foreach(var item in loadedItems)
            {
                for(int i=0;i<item.stackSize;i++)
                {
                    AddItem(item.data);
                }
            }
            return;
        }
        for(int i = 0;i<startingItems.Count;i++)
        {
            if(startingItems[i]!=null)
            {
                AddItem(startingItems[i]);
            }
        }
    }
    public void EquipItem(ItemDataSO _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;
        InventoryItem newItem = new InventoryItem(newEquipment);
        ItemData_Equipment oldEquipment = null;
        foreach(var item in equipmentDictionary)
        {
            if(item.Key.equipmentType == newEquipment.equipmentType)
            {
                oldEquipment = item.Key;
                break;
            }
        }
        if(oldEquipment != null)
        {
            UnequipItem(oldEquipment);
            
        }
        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();
        RemoveItem(_item);
        UpdateSlotUI();
    }

    #region Add Item
    public void AddItem(ItemDataSO _item)
    {
        switch(_item.itemType)
        {
            case ItemType.Equipment:
                if(CanAddItem())
                {
                    AddToInventory(_item);
                }
                break;
            case ItemType.Material:
                AddToStash(_item);
                break;
        }

        UpdateSlotUI();
    }

    private void AddToInventory(ItemDataSO _item)
    {
        if(inventoryDictionary.TryGetValue(_item, out InventoryItem item))
        {
            item.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }
    private void AddToStash(ItemDataSO _item)
    {
        if(stashDictionary.TryGetValue(_item, out InventoryItem item))
        {
            item.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);
            stash.Add(newItem);
            stashDictionary.Add(_item, newItem);
        }
    } 

    public bool CanAddItem()
    {
        if(inventory.Count >= inventoryItemSlots.Length)
        {
            Debug.Log("Inventory is full");
            return false;
        }
        return true;
    }
    #endregion


    #region Remove Item
    public void RemoveItem(ItemDataSO _item)
    {
        switch(_item.itemType)
        {
            case ItemType.Equipment:
                RemoveFromInventory(_item);
                break;
            case ItemType.Material:
                RemoveFromStash(_item);
                break;

        }

        UpdateSlotUI();
    }
    private void RemoveFromInventory(ItemDataSO _item)
    {
        if(inventoryDictionary.TryGetValue(_item, out InventoryItem item))
        {
            item.RemoveStack();
            if(item.stackSize == 0)
            {
                inventory.Remove(item);
                inventoryDictionary.Remove(_item);
            }
        }
    }
    private void RemoveFromStash(ItemDataSO _item)
    {
        if(stashDictionary.TryGetValue(_item, out InventoryItem item))
        {
            item.RemoveStack();
            if(item.stackSize == 0)
            {
                stash.Remove(item);
                stashDictionary.Remove(_item);
            }
        }
    }
    public void RemoveFromEquipment(ItemData_Equipment _item)
    {
        if(equipmentDictionary.TryGetValue(_item, out InventoryItem item))
        {
            equipment.Remove(item);
            equipmentDictionary.Remove(_item);
            _item.RemoveModifiers();
            equipmentSlots[(int)_item.equipmentType].CleanUpSlot();
        }

    }   

    public void UnequipItem(ItemData_Equipment _oldEquipment)
    {
        if(equipmentDictionary.TryGetValue(_oldEquipment, out InventoryItem item))
        {
            equipment.Remove(item);
            equipmentDictionary.Remove(_oldEquipment);
            _oldEquipment.RemoveModifiers();
            AddItem(_oldEquipment);
        }
    }
    #endregion

    public ItemData_Equipment GetEquipmentByType(EquipmentType _equipmentType)
    {
        foreach(var item in equipmentDictionary)
        {
            if(item.Key.equipmentType == _equipmentType)
            {
                return item.Key;
            }
        }
        return null;
}

    public void UseFlask()
    {
        ItemData_Equipment flask = GetEquipmentByType(EquipmentType.Flask);
        if(flask == null)
        {
            Debug.Log("No flask equipped");
            return;
        }
        bool canUseFlask = Time.time > lastTimeUsedFlask + flask.cooldown;
        if(canUseFlask)
        {
            flask.ExecuteItemEffect(null);
            lastTimeUsedFlask = Time.time;
        }
        else
        {
            Debug.Log("Flask is on cooldown");
        }
    }

    public bool CanCraft(ItemData_Equipment _craftData,List<InventoryItem> _craftMaterials)
    {
        foreach(var item in _craftMaterials)
        {
            if(!stashDictionary.TryGetValue(item.data, out InventoryItem inventoryItem)&&!inventoryDictionary.TryGetValue(item.data, out inventoryItem))
            {
                Debug.Log("You don't have the required materials to craft " + _craftData.itemName);
                return false;
            }

        }
        foreach(var item in _craftMaterials)
        {
            RemoveItem(item.data);
        }
        AddItem(_craftData);
        return true;
    }
    public void SaveData(ref GameData _data)
    {
        _data.inventory.Clear();
        foreach(var pair in inventoryDictionary)
        {
            _data.inventory.Add(pair.Key.itemID,pair.Value.stackSize);
        }
        foreach(var pair in stashDictionary)
        {
            _data.inventory.Add(pair.Key.itemID,pair.Value.stackSize);
        }
        foreach(var pair in equipmentDictionary)
        {
            _data.equipmentId.Add(pair.Key.itemID);
        }

    }
    public void LoadData(GameData _data)
    {
        Debug.Log("Loading data");
        
        foreach(var pair in _data.inventory)
        {
            foreach(var item in GetItemDataBase())
            {
                if(item!=null && item.itemID == pair.Key)
                {
                    InventoryItem itemToLoad = new InventoryItem(item);
                    itemToLoad.stackSize = pair.Value;
                    loadedItems.Add(itemToLoad);
                }
            }
        }
        foreach(var id in _data.equipmentId)
        {
            foreach(var item in GetItemDataBase())
            {
                if(item!=null && item.itemID == id)
                {
                    loadedEquipment.Add(item as ItemData_Equipment);
                }
            }
        }
    }

    private List<ItemDataSO> GetItemDataBase()
    {
        List<ItemDataSO> itemDataBase = new List<ItemDataSO>();
        string[] assetNames = AssetDatabase.FindAssets("",new[]{"Assets/Data/Items"});
        foreach(var assetName in assetNames)
        {
            var SOPath= AssetDatabase.GUIDToAssetPath(assetName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemDataSO>(SOPath);
            itemDataBase.Add(itemData);
        }
        return itemDataBase;
    }
}
