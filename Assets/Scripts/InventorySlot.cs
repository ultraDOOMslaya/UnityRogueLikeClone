using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Text amount;

    ItemData item;

    public void AddItem(ItemData newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
        amount.text = item.amount.ToString();
        amount.enabled = true;
    }

    public void ClearSlot()
    {
        icon = null;
        item = null;
        icon.enabled = false;
        amount.text = "-";
        amount.enabled = false;
    }

}
