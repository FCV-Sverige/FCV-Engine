using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Health : MonoBehaviour
{
    public UnityEvent<int, int> onHealthChanged;
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
        onHealthChanged.Invoke(CurrentHealth, maxHealth);
    }
}
