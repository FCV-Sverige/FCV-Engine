using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class CustomAnimator : MonoBehaviour
{
    [SerializeField] private List<Sprite> animationSprites;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    /// <summary>
    /// Changes the sprite of SpriteRenderer depending on a percentile
    /// </summary>
    /// <param name="percentile">a value from 0 to 1</param>
    public void PercentileAnimation(float percentile)
    {
        int animationIndex = Mathf.FloorToInt(animationSprites.Count * percentile);
        animationIndex = Mathf.Clamp(animationIndex, 0, animationSprites.Count - 1);

        spriteRenderer.sprite = animationSprites[animationIndex];
    }
}
