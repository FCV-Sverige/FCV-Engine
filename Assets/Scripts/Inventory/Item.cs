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
}
