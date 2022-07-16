using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public List<ItemData> items = new List<ItemData>();
    public int space = 20;
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    void Awake()
    {
        instance = this;
    }

    public bool Add(ItemData itemData, int amount)
    {
        if (items.Count >= space)
        {
            Debug.Log("Inventory is full");
            return false;
        }

        if (items.Exists(item => item.name == itemData.name))
        {
            items[items.FindIndex(item => item.name == itemData.name)].amount += amount;
        }
        else
        {
            itemData.amount = amount;
            items.Add(itemData);
        }
        
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
        return true;
    }

    public void Remove(ItemData item)
    {
        items.Remove(item);
        if (onItemChangedCallback != null)
        {
            onItemChangedCallback.Invoke();
        }
    }
}
