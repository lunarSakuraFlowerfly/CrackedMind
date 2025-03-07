using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    #region 背包可见或隐藏
    private void Hide()
    {
        backpack?.SetActive(false);
    }
    private void Show()
    {
        backpack?.SetActive(true);
    }
    #endregion

    public void AddItem(ItemSO itemSO)
    {
        GameObject itemGO = Instantiate(itemPrefab, content.transform);
        ItemUI itemUI = itemGO.GetComponent<ItemUI>();
        itemUI?.InitItem(itemSO);
    }

    public void ShowItemDetailUI(ItemSO itemSO, ItemUI itemUI)
    {
        itemDetailUI.GetComponent<ItemDetailUI>().UpdateDetailUI(itemSO,itemUI);
    }

    public void UseItem(ItemSO itemSO, ItemUI itemUI)
    {
        Destroy(itemUI.gameObject);
        itemDetailUI.SetActive(false);
        PlayerController.Instance.UseItem(itemSO);
    }
    public void ThrowItem(ItemSO itemSO, ItemUI itemUI)
    {
        Destroy(itemUI.gameObject);
        itemDetailUI.SetActive(false);
        GameObject itemGO = Instantiate(itemSO.prefab);
        itemGO.transform.position = new Vector3(PlayerController.Instance.gameObject.transform.position.x+0.8f,PlayerController.Instance.gameObject.transform.position.y,0);
        if(itemGO.GetComponent<PickableObject>() == null)
        {
            itemGO.AddComponent<PickableObject>();
            itemGO.GetComponent<PickableObject>().itemSO = itemSO;
        }
    }
}
