using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Dictionary<string, Item> InventoryItems { get; set; } = new();

    private void Awake()
    {
        InventoryItems = new();
    }

    public void TryAddItem(string itemName, Item item)
    {
        InventoryItems.TryAdd(itemName, item);
    }

    public bool TryRemoveItem(string itemName)
    {
        return InventoryItems.Remove(itemName);
    }

    public bool HasItem(string itemName)
    {
        return InventoryItems.ContainsKey(itemName);
    }
}
