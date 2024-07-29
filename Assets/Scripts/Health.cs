using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    
    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
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
        currentHealth += healthChange;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
}
