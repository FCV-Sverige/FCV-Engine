using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    
    private int health;

    private void Awake()
    {
        health = maxHealth;
    }

    public void RemoveHealth(int amount)
    {
        if (amount > 0) amount *= -1;
        ChangeHealth(amount);
    }

    public void AddHealth(int amount)
    {
        if (amount < 0) amount *= -1;
        ChangeHealth(amount);
    }

    private void ChangeHealth(int healthChange)
    {
        health += healthChange;
        health = Mathf.Clamp(health, 0, maxHealth);
    }
}
