using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class Item : MonoBehaviour
{
    [ItemName, SerializeField] private string itemName;
    [SerializeField] private Sprite sprite;

    public string ItemName => itemName;

    private ItemDatabase itemDatabase;
    
    /// <summary>
    /// Applies specified sprite automaticly from sprite variable
    /// </summary>
    private void OnValidate()
    {
        if (TryGetComponent(out SpriteRenderer spriteRenderer))
            spriteRenderer.sprite = sprite;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Adds item to inventory and deactivates the gameobject
        if (!other.TryGetComponent(out Inventory inventory)) return;
        inventory.TryAddItem(itemName, this);
        gameObject.SetActive(false);
    }
}
