using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private UnityEvent<float> healthUpdate;

    private void Awake()
    {
        if (!health) return;
        
        health.onHealthChanged.AddListener(OnHealthChanged);
    }

    private void OnDisable()
    {
        health.onHealthChanged.RemoveListener(OnHealthChanged);
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        healthUpdate.Invoke(currentHealth / (float)maxHealth);
    }
}
