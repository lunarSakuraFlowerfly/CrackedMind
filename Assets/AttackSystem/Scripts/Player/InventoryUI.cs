using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractionSystem.Data;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }
    public GameObject content;
    public GameObject itemPrefab;
    public GameObject backpack;
    public GameObject itemDetailUI;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        Hide();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            backpack.SetActive(!backpack.activeSelf);
        }
    }
    #region 背包可见性控制
    private void Hide()
    {
        backpack?.SetActive(false);
    }
    private void Show()
    {
        backpack?.SetActive(true);
    }
    #endregion

    public void AddItem(ItemData itemData)
    {
        GameObject itemGO = Instantiate(itemPrefab, content.transform);
        ItemUI itemUI = itemGO.GetComponent<ItemUI>();
        itemUI?.InitItem(itemData);
    }

    public void ShowItemDetailUI(ItemData itemData, ItemUI itemUI)
    {
        itemDetailUI.GetComponent<ItemDetailUI>().UpdateDetailUI(itemData,itemUI);
    }

    public void UseItem(ItemData itemData, ItemUI itemUI)
    {
        Destroy(itemUI.gameObject);
        itemDetailUI.SetActive(false);
        PlayerController.Instance.UseItem(itemData);
    }
    public void ThrowItem(ItemData itemData, ItemUI itemUI)
    {
        Destroy(itemUI.gameObject);
        itemDetailUI.SetActive(false);
        GameObject itemGO = Instantiate(itemData.prefab);
        itemGO.transform.position = new Vector3(PlayerController.Instance.gameObject.transform.position.x+0.8f,PlayerController.Instance.gameObject.transform.position.y,0);
        
        // 添加可拾取组件
        var pickable = itemGO.GetComponent<InteractionSystem.Item.ItemInteractable>();
        if(pickable == null)
        {
            pickable = itemGO.AddComponent<InteractionSystem.Item.ItemInteractable>();
            pickable.SetItemData(itemData);
        }
    }
}
