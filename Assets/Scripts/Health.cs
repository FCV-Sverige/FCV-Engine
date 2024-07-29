using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;

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
    }
}
