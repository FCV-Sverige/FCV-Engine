using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tools/ItemDatabase", fileName = "ItemNameDatabase"), ]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<string> itemNames;

    public List<string> GetAllItemNames()
    {
        return itemNames;
    }

    public string GetName(int index)
    {
        return itemNames[index];
    }

    public void UpdateName(int index, string itemName)
    {
        if (index > itemNames.Count)
        {
            Debug.LogWarning($"{itemName} already exists in database");
            return;
        }

        itemNames[index] = itemName;
    }

    public bool HasItem(string itemName)
    {
        return itemNames.Contains(itemName);
    }

    public void AddItem(string itemName)
    {
        if (itemNames.Contains(itemName))
        {
            Debug.LogWarning($"{itemName} already exists in database");
            return;
        }
            
        itemNames.Add(itemName);
    }

    public void ClearItems()
    {
        itemNames.Clear();
    }
}