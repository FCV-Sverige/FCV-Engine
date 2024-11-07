using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Health : MonoBehaviour
{
    public UnityEvent<int, int> onHealthChanged;
    public UnityEvent<GameObject> onDeath;
    [SerializeField] private int maxHealth;
    [SerializeField] private bool destroyOnDeath = false;

    public int CurrentHealth { get; private set; }

    public int MaxHealth => maxHealth;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void RemoveHealth(int amount)
    {
        if (amount > 0) amount *= -1;
        ModifyHealth(amount);
    }

    public void AddHealth(int amount)
    {
        if (amount < 0) amount *= -1;
        ModifyHealth(amount);
    }

    private void ModifyHealth(int healthChange)
    {
        CurrentHealth += healthChange;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
        if (CurrentHealth > 0)
        {
            onHealthChanged.Invoke(CurrentHealth, maxHealth);
        }
        else
        {
            onDeath.Invoke(this.gameObject);
            
            if (destroyOnDeath)
                Destroy(gameObject);
        }
    }
}
