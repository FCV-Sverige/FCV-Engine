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

    private void OnValidate()
    {
        if (TryGetComponent(out SpriteRenderer spriteRenderer))
            spriteRenderer.sprite = sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        print("Added item to inventory");
        if (!other.TryGetComponent(out Inventory inventory)) return;
        inventory.TryAddItem(itemName, this);
        gameObject.SetActive(false);
    }
}
