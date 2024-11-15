using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Knockback : MonoBehaviour
{
    [SerializeField] private float knockbackFactor = 2f;
    
    private Rigidbody2D rb2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    public void ApplyForce(Vector2 direction, float amount)
    {
        amount = Mathf.Clamp01(amount);
        rb2D.AddForce((direction.normalized + Vector2.up).normalized * (knockbackFactor * amount), ForceMode2D.Impulse);
    }
}
