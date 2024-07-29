using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Damager : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private LayerMask layerMask;
    
    private void DealDamage(Health health)
    {
        if (!health)
        {
            Debug.Log("Health component is null");
            return;
        }
        
        health.RemoveHealth(damage);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!LayerMaskUtility.IsInLayerMask(other.gameObject, layerMask)) return;

        if (other.TryGetComponent(out Health health))
        {
            DealDamage(health);
        }
    }
}
