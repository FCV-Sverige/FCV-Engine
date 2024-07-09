using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Dictionary<string, Item> inventoryItems = new();
    
    public Dictionary<string, Item> InventoryItems => inventoryItems;


    public void TryAddItem(string itemName, Item item)
    {
        inventoryItems.TryAdd(itemName, item);
    }

    public bool TryRemoveItem(string itemName)
    {
        return inventoryItems.Remove(itemName);
    }

    public bool HasItem(string itemName)
    {
        return inventoryItems.ContainsKey(itemName);
    }
}
