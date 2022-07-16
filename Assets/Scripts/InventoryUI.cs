using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public GameObject inventoryUI;

    private InventoryManager _inventory;
    private InventorySlot[] _slots;

    // Start is called before the first frame update
    void Start()
    {
        _inventory = InventoryManager.instance;
        _inventory.onItemChangedCallback += UpdateUI;

        _slots = itemsParent.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (i < _inventory.items.Count)
            {
                _slots[i].AddItem(_inventory.items[i]);
            }
            //TODO add an else that handles removing inventory items
        }
        Debug.Log("Updating UI");
    }
}
